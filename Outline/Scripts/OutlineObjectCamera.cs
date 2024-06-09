using MustHave.Utils;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif

namespace MustHave
{
    [RequireComponent(typeof(Camera))]
    public class OutlineObjectCamera : MonoBehaviour
    {
        public const string PrefabName = "OutlineObjectCamera";

        private const int RenderersCapacity = 1 << 10;

        public readonly struct Layer
        {
            public const string OutlineLayerName = "Outline-MustHave";

            public static int OutlineLayer = LayerMask.NameToLayer(OutlineLayerName);
            public static int OutlineMask = LayerMask.GetMask(OutlineLayerName);

            public static void Refresh()
            {
                OutlineLayer = LayerMask.NameToLayer(OutlineLayerName);
                OutlineMask = LayerMask.GetMask(OutlineLayerName);
            }
        }

        public ComputeShader ComputeShader => shaderSettings.Shader;
        public OutlineShaderSettings ShaderSettings => shaderSettings;
        public RenderTexture ShapeTexture => shapeTexture;
        public RenderTexture CircleTexture => circleTexture;
        public Material OutlineShapeMaterial => outlineShapeMaterial;
        public Camera ShapeCamera => shapeCamera;
        public Camera CircleCamera => circleCamera;
        public int ObjectsCount => objects.Count;

        private int RenderersCount => Mathf.Min(RenderersCapacity, renderersData.Count);

        [SerializeField]
        private OutlineShaderSettings shaderSettings = null;
        [SerializeField]
        private Material outlineShapeMaterial = null;
        [SerializeField]
        private Material circleSpriteMaterial = null;
        [SerializeField]
        private MeshFilter quadMeshFilter = null;
        [SerializeField]
        private Camera circleCamera = null;
#if UNITY_EDITOR
        [SerializeField]
        private bool layerAdded = false;
#endif
        private Camera shapeCamera = null;

        private RenderTexture shapeTexture = null;
        private RenderTexture circleTexture = null;

        private readonly List<OutlineObject> objects = new();
        private readonly List<RendererData> renderersData = new();

        private readonly InstanceData[] circleInstanceData = new InstanceData[RenderersCapacity];
        private readonly Material[] shapeMaterials = new Material[RenderersCapacity];

        private GraphicsBuffer circleInstanceBuffer = null;
        private MaterialPropertyBlock circlePropertyBlock = null;
        private RenderParams circleRenderParams = default;
        //private GraphicsBuffer circleCommandBuffer = null;
        //private IndirectDrawIndexedArgs[] circleCommandData = null;
        //private Matrix4x4[] circleMatrices = null;

        private struct InstanceData
        {
            public Matrix4x4 objectToWorld;
            public Vector3 clipPosition;
            public Vector4 color;
            public Vector2 scale;
        }

        public void DestroyUniversalAdditionalCameraData()
        {
#if UNITY_PIPELINE_URP
            var componens = GetComponentsInChildren<UniversalAdditionalCameraData>();
            foreach (var cameraData in componens)
            {
                ObjectUtils.DestroyComponent(cameraData);
            }
#endif
        }

        public void CreateRuntimeAssets(Vector2Int texSize)
        {
            CreateTextures(texSize);

            CreateMissingShapeMaterials(10);

            InitCircleInstancing();
        }

        public void DestroyRuntimeAssets()
        {
            ReleaseTexture(ref shapeTexture);
            ReleaseTexture(ref circleTexture);

            circleInstanceBuffer?.Release();
            circleInstanceBuffer = null;
            //circleCommandBuffer?.Release();
            //circleCommandBuffer = null;
        }

        public void Setup(OutlineCamera outlineCamera, bool copySettings = true)
        {
#if UNITY_EDITOR
            if (!layerAdded)
            {
                layerAdded = UnityUtils.AddLayer(Layer.OutlineLayerName, out bool layerExists) || layerExists;

                if (layerAdded)
                {
                    Layer.Refresh();

                    if (!Application.isPlaying)
                    {
                        UnityEditor.EditorUtility.SetDirty(this);
                    }
                }
            }
#endif
            var parentCamera = outlineCamera.Camera;
            shapeCamera = GetComponent<Camera>();

            if (copySettings)
            {
                shapeCamera.CopyFrom(parentCamera);
                shapeCamera.fieldOfView = GetExtendedFieldOfView(parentCamera, OutlineCamera.LineMaxThickness);
                shapeCamera.orthographicSize = GetExtendedOrthoSize(parentCamera, OutlineCamera.LineMaxThickness);
                //Debug.Log($"{GetType().Name}.Setup: srcFov: {parentCamera.fieldOfView:f2} dstFov: {shapeCamera.fieldOfView:f2}");
            }
            shapeCamera.targetTexture = shapeTexture;
            shapeCamera.clearFlags = CameraClearFlags.SolidColor;
            shapeCamera.backgroundColor = Color.clear;
            shapeCamera.cullingMask = Layer.OutlineMask;
            shapeCamera.allowMSAA = false;
            shapeCamera.enabled = outlineCamera.PipelineType != RenderPipelineType.Default;
            shapeCamera.depthTextureMode = DepthTextureMode.Depth;
            shapeCamera.nearClipPlane = Mathf.Min(parentCamera.nearClipPlane, 0.001f);

            if (copySettings)
            {
                circleCamera.CopyFrom(shapeCamera);
            }
            circleCamera.targetTexture = circleTexture;
            circleCamera.orthographic = true;
            circleCamera.cullingMask = Layer.OutlineMask;
            circleCamera.enabled = true;

#if UNITY_PIPELINE_URP
            if (outlineCamera.PipelineType == RenderPipelineType.URP)
            {
                SetupURPCamera(shapeCamera);
                SetupURPCamera(circleCamera);
            }
#endif
        }

        public void AddOutlineObject(OutlineObject obj)
        {
            if (!objects.Contains(obj))
            {
                objects.Add(obj);
                obj.ForEachRendererData(data => {
                    renderersData.Add(data);
                });
            }
        }

        public void RemoveOutlineObject(OutlineObject obj)
        {
            objects.Remove(obj);
            obj.ForEachRendererData(data => {
                renderersData.Remove(data);
            });
            if (objects.Count == 0)
            {
                circleTexture.Clear();
                shapeTexture.Clear();
            }
        }

#if UNITY_PIPELINE_URP
        private void SetupURPCamera(Camera camera)
        {
            var cameraData = camera.GetOrAddUniversalAdditionalCameraData();

            cameraData.requiresColorOption = CameraOverrideOption.On;
            cameraData.requiresDepthOption = CameraOverrideOption.On;
        }
#endif

        private float GetExtendedOrthoSize(Camera parentCamera, int pixelOffset)
        {
            float orthoHalfHeight = parentCamera.orthographicSize;
            float w = parentCamera.pixelWidth;
            float h = parentCamera.pixelHeight;

            if (w > h)
            {
                return orthoHalfHeight * (1f + 2f * pixelOffset / parentCamera.pixelHeight);

            }
            else
            {
                //float orthoHalfWidth = orthoHalfHeight * w / h;
                //float destOrthoHalfWidth = orthoHalfWidth * (1f + 2f * pixelOffset / parentCamera.pixelWidth);
                //float destOrthoHalfHeight = destOrthoHalfWidth * h / w;
                return orthoHalfHeight * (1f + 2f * pixelOffset / parentCamera.pixelWidth);
            }
        }

        private float GetExtendedFieldOfView(Camera parentCamera, int pixelOffset)
        {
            // tanHalfFov = 0.5 * h / r
            // r = 0.5 * h / tanHalfFov
            // r = 0.5 * (h + dh) / tanHalfFov2
            // h / tanHalfFov = (h + dh) / tanHalfFov2
            // h / (h + dh) = tanHalfFov / tanHalfFov2
            // tanHalfFov2 = tanHalfFov * (h + dh) / h

            float aspect = parentCamera.aspect;
            float w = parentCamera.pixelWidth;
            float h = parentCamera.pixelHeight;
            float destFovY;

            if (w > h)
            {
                float dh = pixelOffset << 1;
                float tanHalfFovY = parentCamera.GetTanHalfFovVerti();
                float destHalfFovY = Mathf.Atan2(tanHalfFovY * (h + dh), h);

                destFovY = destHalfFovY * 2f * Mathf.Rad2Deg;
            }
            else
            {
                float dw = pixelOffset << 1;
                float tanHalfFovX = parentCamera.GetTanHalfFovHori();
                float destHalfFovX = Mathf.Atan2(tanHalfFovX * (w + dw), w);

                float destFovX = destHalfFovX * 2f * Mathf.Rad2Deg;
                destFovY = Camera.HorizontalToVerticalFieldOfView(destFovX, aspect);
            }
            return destFovY;
        }


        private void CreateTextures(Vector2Int texSize)
        {
            shapeTexture = CreateTexture(texSize, "OutlineObjectsShapeTexture");
            circleTexture = CreateTexture(texSize, "OutlineObjectsCircleTexture");
            circleTexture.filterMode = FilterMode.Point;
        }

        private void CreateMissingShapeMaterials(int excess)
        {
            int matIndex = Mathf.Clamp(renderersData.Count - 1, 0, shapeMaterials.Length - 1);
            if (!shapeMaterials[matIndex])
            {
                matIndex = Mathf.Clamp(renderersData.Count + excess - 1, 0, shapeMaterials.Length - 1);
                while (matIndex >= 0 && !shapeMaterials[matIndex])
                {
                    //Debug.Log($"{GetType().Name}.CreateMissingShapeMaterials: {matIndex}");
                    shapeMaterials[matIndex--] = new Material(outlineShapeMaterial);
                }
            }
        }

        private void InitCircleInstancing()
        {
            circleInstanceBuffer?.Release();
            circleInstanceBuffer = new GraphicsBuffer(Target.Structured, circleInstanceData.Length, Marshal.SizeOf<InstanceData>());
            circlePropertyBlock = new MaterialPropertyBlock();
            circlePropertyBlock.SetBuffer("_InstanceBuffer", circleInstanceBuffer);
            circleRenderParams = new RenderParams(circleSpriteMaterial)
            {
                camera = circleCamera,
                matProps = circlePropertyBlock,
                layer = Layer.OutlineLayer,
                //renderingLayerMask = (uint)Layer.OutlineMask,
                worldBounds = new Bounds(Vector3.zero, Vector3.one)
            };
            //circleCommandBuffer = new GraphicsBuffer(Target.IndirectArguments, 1, IndirectDrawIndexedArgs.size);
            //circleCommandData = new IndirectDrawIndexedArgs[1];
            //circleMatrices = new Matrix4x4[RenderersCapacity];
        }

        private RenderTexture CreateTexture(Vector2Int size, string name = "")
        {
            var texture = new RenderTexture(size.x, size.y, 0)
            {
                name = name,
                enableRandomWrite = true
            };
            texture.Create();
            return texture;
        }

        private void ReleaseTexture(ref RenderTexture texture)
        {
            if (texture)
            {
                texture.Release();
                texture = null;
            }
        }

        private void SortRenderers()
        {
            foreach (var data in renderersData)
            {
                data.GetDistanceFromCamera(shapeCamera.transform.position);
            }
            renderersData.Sort((a, b) => a.CameraDistanceSqr.CompareTo(b.CameraDistanceSqr));
        }

        private void SetSortedRenderersDepth()
        {
            int count = renderersData.Count;
            if (count <= 0)
            {
                return;
            }
            for (int i = 1; i <= count; i++)
            {
                renderersData[i - 1].Depth = (float)i / count;
            }
        }

        public void OnBeginRenderingShapes()
        {
            shapeTexture.Clear();

            int count = RenderersCount;
            if (count <= 0)
            {
                return;
            }
            CreateMissingShapeMaterials(10);

            float minDepth = 1f / count;

            // At this point renderers are sorted by distance from camera
            for (int i = 0; i < count; i++)
            {
                renderersData[i].Setup(shapeMaterials[i], Layer.OutlineLayer, minDepth);
            }
        }

        public void OnEndRenderingShapes()
        {
            foreach (var data in renderersData)
            {
                data.Restore();
            }
        }

        public void RenderShapes()
        {
            OnBeginRenderingShapes();

            shapeCamera.Render();

            OnEndRenderingShapes();
        }

        public void RenderCircles(int radius/*, CommandBuffer cmd = null*/)
        {
            circleTexture.Clear();

            int count = RenderersCount;
            if (count <= 0)
            {
                return;
            }
            int layer = circleRenderParams.layer;
            if (layer < 0 || layer > 31)
            {
                return;
            }
            var circlesCamTransform = circleCamera.transform;

            float scale = 2f * (radius + 1) / circleCamera.pixelHeight;
            var scaleXY = scale * Vector2.one;
            float minDepth = 1f / count;

            // At this point renderers are sorted by distance from camera
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if (SetCircleInstanceData(j, scaleXY, minDepth))
                {
                    j++;
                }
            }
            count = j;
            if (count > 0)
            {
                circleInstanceBuffer.SetData(circleInstanceData, 0, 0, count);
                circleRenderParams.material.SetFloat("_MinDepth", minDepth);
                circleRenderParams.worldBounds = new Bounds(circlesCamTransform.position, Vector3.one);

                //circleCommandData[0].indexCountPerInstance = quadMeshFilter.sharedMesh.GetIndexCount(0);
                //circleCommandData[0].instanceCount = (uint)count;
                //circleCommandBuffer.SetData(circleCommandData);

                //if (cmd != null)
                //{
                //    //cmd.DrawMeshInstanced(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial, 0, circleMatrices, count, circlePropertyBlock);
                //    //cmd.DrawMeshInstancedProcedural(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial, 0, count, circlePropertyBlock);
                //    //cmd.DrawMeshInstancedIndirect(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial, 0, circleCommandBuffer, 0, circlePropertyBlock);
                //}
                //else
                {
                    Graphics.RenderMeshPrimitives(circleRenderParams, quadMeshFilter.sharedMesh, 0, count);
                    //Graphics.RenderMeshIndirect(circleRenderParams, quadMeshFilter.sharedMesh, circleCommandBuffer);
                    //Graphics.RenderMeshInstanced(circleRenderParams, quadMeshFilter.sharedMesh, 0, circleInstanceData, count);
                    //Graphics.DrawMeshInstancedProcedural(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial,
                    //    circleRenderParams.worldBounds, count, circlePropertyBlock,
                    //    UnityEngine.Rendering.ShadowCastingMode.Off, false, Layer.OutlineLayer, circleCamera);
                }

            }
        }

        private bool SetCircleInstanceData(int i, Vector2 scale, float minDepth)
        {
            var data = renderersData[i];
            var renderer = data.Renderer;
            var center = renderer.bounds.center;
            var viewPoint = shapeCamera.WorldToViewportPoint(center);

            if (!CameraUtils.IsBetweenNearAndFar(circleCamera, viewPoint.z))
            {
                return false;
            }
            //Debug.Log($"{GetType().Name}.SetCircleInstanceData: {viewPoint}");
            //var worldPoint = circlesCamera.ViewportToWorldPoint(viewPoint);
            //objectToWorld.SetTRS(worldPoint, Quaternion.LookRotation(circlesCamTransform.forward, circlesCamTransform.up), Vector3.one);
            var clipPoint = new Vector3()
            {
                x = (viewPoint.x - 0.5f) * 2f,
                y = (viewPoint.y - 0.5f) * 2f,
                z = data.Depth
            };
            var color = data.GetColorWithAlphaDepth(minDepth);
            //Debug.Log($"{GetType().Name}.{i} | {data.CameraDistanceSqr} | {clipPoint.z}");
            circleInstanceData[i] = new InstanceData()
            {
                objectToWorld = Matrix4x4.identity,
                clipPosition = clipPoint,
                color = color,
                scale = scale
            };
            return true;
        }

        public void OnLateUpdate(/*OutlineCamera outlineCamera*/)
        {
            foreach (var obj in objects)
            {
                obj.SetRenderersColor();
            }
            SortRenderers();
            SetSortedRenderersDepth();
        }

        private void OnDrawGizmos()
        {
            //foreach (OutlineObject obj in objects)
            //{
            //    obj.DrawBBoxGizmo();
            //}
        }

        private void OnDestroy()
        {
            DestroyRuntimeAssets();
        }
    }
}
