using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;
using CircleShaderVariant = MustHave.OutlineShaderSettings.CircleShaderVariant;
#if UNITY_PIPELINE_HDRP
using static UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData;
using UnityEngine.Rendering.HighDefinition;
#endif
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif

namespace MustHave
{
    [RequireComponent(typeof(Camera))]
    public class OutlineObjectCamera : MonoBehaviour
    {
        public const string PrefabPath = "Assets/Packages/MustHave/Outline/Prefabs/Resources/OutlineObjectCamera.prefab";
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
        public CircleShaderVariant CircleShaderVariant => shaderSettings.CirclesShaderVariant;
        public OutlineShaderSettings ShaderSettings => shaderSettings;
        public RenderTexture ShapeTexture => shapeTexture;
        public RenderTexture CircleTexture => circleTexture;
        public Material OutlineShapeMaterial => outlineShapeMaterial;
        public Camera ShapeCamera => shapeCamera;
        public Camera CircleCamera => circleCamera;
        public int ObjectsCount => Mathf.Min(RenderersCapacity, objects.Count);

        [SerializeField]
        private OutlineShaderSettings shaderSettings = null;
        [SerializeField]
        private Material outlineShapeMaterial = null;
        [SerializeField]
        private Material circleSpriteMaterial = null;
        [SerializeField]
        private MeshFilter quadMeshFilter = null;
        [SerializeField]
        private Camera shapeCamera = null;
        [SerializeField]
        private Camera circleCamera = null;
        [SerializeField, HideInInspector]
        private ColorSpace colorSpace = ColorSpace.Uninitialized;
#if UNITY_EDITOR
        [SerializeField]
        private bool layerAdded = false;
#endif

        private RenderTexture shapeTexture = null;
        private RenderTexture circleTexture = null;

        private readonly List<OutlineObject> objects = new();

        private readonly InstanceData[] circleInstanceData = new InstanceData[RenderersCapacity];
        private readonly Matrix4x4[] circleObjectToWorld = new Matrix4x4[RenderersCapacity];
        private readonly Color[] circleColor = new Color[RenderersCapacity];
        private readonly Material[] shapeMaterials = new Material[RenderersCapacity];

        private GraphicsBuffer circleInstanceBuffer = null;
        private GraphicsBuffer circleColorBuffer = null;
        private MaterialPropertyBlock circlePropertyBlock = null;
        private RenderParams circleRenderParams = default;
        //private GraphicsBuffer circleCommandBuffer = null;
        //private IndirectDrawIndexedArgs[] circleCommandData = null;

        private struct InstanceData
        {
            public Vector3 clipPosition;
            public Vector4 color;
            public Vector2 scale;
        }

        public void DestroyAdditionalCameraData()
        {
#if UNITY_PIPELINE_HDRP
            this.DestroyComponentsInChilden<HDAdditionalCameraData>();
#endif
#if UNITY_PIPELINE_URP
            this.DestroyComponentsInChilden<UniversalAdditionalCameraData>();
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
            circleColorBuffer?.Release();
            circleColorBuffer = null;
            //circleCommandBuffer?.Release();
            //circleCommandBuffer = null;
        }

        public void ClearTextures()
        {
            circleTexture.Clear();
            shapeTexture.Clear();
        }

        public void Setup(OutlineCamera outlineCamera, bool copySettings = true)
        {
#if UNITY_EDITOR
            bool setDirty = false;

            if (!layerAdded)
            {
                layerAdded = UnityUtils.AddLayer(Layer.OutlineLayerName, out bool layerExists) || layerExists;

                if (layerAdded)
                {
                    Layer.Refresh();

                    setDirty = true;
                }
            }
            if (colorSpace != UnityEditor.PlayerSettings.colorSpace)
            {
                colorSpace = UnityEditor.PlayerSettings.colorSpace;

                setDirty = true;
            }
            if (setDirty && !Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
            var parentCamera = outlineCamera.Camera;

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

            SetupCameraAdditionalData(outlineCamera.PipelineType);
        }

        public void AddOutlineObject(OutlineObject obj)
        {
            if (!objects.Contains(obj))
            {
                objects.Add(obj);
            }
        }

        public void RemoveOutlineObject(OutlineObject obj)
        {
            objects.Remove(obj);

            if (objects.Count == 0)
            {
                circleTexture.Clear();
                shapeTexture.Clear();
            }
        }

        public void SetupCameraAdditionalData(RenderPipelineType pipelineType)
        {
            SetupCameraAdditionalData(shapeCamera, pipelineType);
            SetupCameraAdditionalData(circleCamera, pipelineType);
        }

        private void SetupCameraAdditionalData(Camera camera, RenderPipelineType pipelineType)
        {
            switch (pipelineType)
            {
                case RenderPipelineType.Default:
                    break;
#if UNITY_PIPELINE_URP
                case RenderPipelineType.URP:
                {
                    var cameraData = camera.GetOrAddUniversalAdditionalCameraData();

                    cameraData.requiresColorOption = CameraOverrideOption.On;
                    cameraData.requiresDepthOption = CameraOverrideOption.On;
                }
                break;
#endif
#if UNITY_PIPELINE_HDRP
                case RenderPipelineType.HDRP:
                {
                    var cameraData = camera.GetOrAddHDAdditionalCameraData();

                    cameraData.clearColorMode = ClearColorMode.Color;
                    cameraData.backgroundColorHDR = ColorUtils.ColorWithAlpha(cameraData.backgroundColorHDR, 0f);
                    cameraData.volumeLayerMask = 0;
                    cameraData.probeLayerMask = 0;
                }
                break;
#endif
                case RenderPipelineType.CustomSRP:
                    break;
                default:
                    break;
            }
        }

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
            int matIndex = Mathf.Clamp(objects.Count - 1, 0, shapeMaterials.Length - 1);
            if (!shapeMaterials[matIndex])
            {
                matIndex = Mathf.Clamp(objects.Count + excess - 1, 0, shapeMaterials.Length - 1);
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
            circleColorBuffer?.Release();
            circleColorBuffer = new GraphicsBuffer(Target.Structured, circleColor.Length, Marshal.SizeOf<Color>());
            circlePropertyBlock = new MaterialPropertyBlock();
            circlePropertyBlock.SetBuffer("_InstanceBuffer", circleInstanceBuffer);
            circlePropertyBlock.SetBuffer("_ColorBuffer", circleColorBuffer);
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

            shaderSettings.SetCircleShaderVariant();
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

        private void SortOutlineObjects()
        {
            foreach (var obj in objects)
            {
                obj.GetDistanceFromCamera(shapeCamera.transform.position);
            }
            objects.Sort((a, b) => a.CameraDistanceSqr.CompareTo(b.CameraDistanceSqr));
        }

        private void SetSortedObjectsColorWithDepth()
        {
            int count = objects.Count;
            if (count <= 0)
            {
                return;
            }
            float minDepth = 1f / count;

            for (int i = 1; i <= count; i++)
            {
                float depth = (float)i / count;

                objects[i - 1].SetColorWithDepth(depth, minDepth);
            }
        }

        public void OnBeginRenderingShapes()
        {
            shapeTexture.Clear();

            int count = ObjectsCount;
            if (count <= 0)
            {
                return;
            }
            CreateMissingShapeMaterials(10);

            float minDepth = 1f / count;

            // At this point objects are sorted by distance from camera
            for (int i = 0; i < count; i++)
            {
                objects[i].SetupRenderers(shapeMaterials[i], Layer.OutlineLayer, minDepth);
            }
        }

        public void OnEndRenderingShapes()
        {
            foreach (var obj in objects)
            {
                obj.RestoreRenderers();
            }
        }

        public void RenderShapes()
        {
            OnBeginRenderingShapes();

            shapeCamera.Render();

            OnEndRenderingShapes();
        }

        public void RenderCircles(int radius, CommandBuffer cmd = null)
        {
            circleTexture.Clear();

            int objectsCount = ObjectsCount;
            if (objectsCount <= 0)
            {
                return;
            }
            int layer = circleRenderParams.layer;
            if (layer < 0 || layer > 31)
            {
                return;
            }
            var circlesCamTransform = circleCamera.transform;

            float normScale = 2f * Mathf.Max(1, radius + 1) / circleCamera.pixelHeight;
            float camScale = 2f * normScale * circleCamera.orthographicSize;
            var normScaleXYZ = new Vector3(normScale, normScale, 1f);
            var camScaleXYZ = new Vector3(camScale, camScale, 1f);
            float minDepth = objectsCount > 0 ? 1f / objectsCount : 0f;

            var circleRotation = Quaternion.LookRotation(circlesCamTransform.forward, circlesCamTransform.up);
            var scaledLookAtCamera = Matrix4x4.TRS(Vector3.zero, circleRotation, camScaleXYZ);

            var getColor = ColorUtils.GetColorFromColorSpaceFunc(colorSpace);

            // At this point objects are sorted by distance from camera
            int j = 0;
            for (int i = objectsCount - 1; i >= 0; i--)
            {
                var obj = objects[i];
                var color = getColor(obj.ColorWithDepth);
                float depth = obj.Depth;

                obj.ForEachRendererData(data => {
                    if (SetCircleInstanceData(data, j, normScaleXYZ, depth, color, scaledLookAtCamera))
                    {
                        j++;
                    }
                });
            }
            int count = j;
            if (count > 0)
            {
                if (CircleShaderVariant == CircleShaderVariant.INSTANCE_DATA_VARIANT)
                {
                    circleInstanceBuffer.SetData(circleInstanceData, 0, 0, count);
                }
                else if (CircleShaderVariant == CircleShaderVariant.INSTANCE_MATRIX_VARIANT)
                {
                    circleColorBuffer.SetData(circleColor, 0, 0, count);
                }
                circleRenderParams.material.SetFloat("_MinDepth", minDepth);
                circleRenderParams.worldBounds = new Bounds(circlesCamTransform.position, Vector3.one);

                //circleCommandData[0].indexCountPerInstance = quadMeshFilter.sharedMesh.GetIndexCount(0);
                //circleCommandData[0].instanceCount = (uint)count;
                //circleCommandBuffer.SetData(circleCommandData);

                if (cmd != null)
                {
                    cmd.ClearRenderTarget(true, true, Color.clear);

                    cmd.DrawMeshInstanced(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial, 0, circleObjectToWorld, count, circlePropertyBlock);
                    //cmd.DrawMeshInstancedProcedural(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial, 0, count, circlePropertyBlock);
                    //cmd.DrawMeshInstancedIndirect(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial, 0, circleCommandBuffer, 0, circlePropertyBlock);
                }
                else
                {
                    Graphics.RenderMeshInstanced(circleRenderParams, quadMeshFilter.sharedMesh, 0, circleObjectToWorld, count);
                    //Graphics.RenderMeshPrimitives(circleRenderParams, quadMeshFilter.sharedMesh, 0, count);
                    //Graphics.RenderMeshIndirect(circleRenderParams, quadMeshFilter.sharedMesh, circleCommandBuffer);
                    //Graphics.DrawMeshInstancedProcedural(quadMeshFilter.sharedMesh, 0, circleSpriteMaterial,
                    //    circleRenderParams.worldBounds, count, circlePropertyBlock,
                    //    UnityEngine.Rendering.ShadowCastingMode.Off, false, Layer.OutlineLayer, circleCamera);
                }
            }
        }

        private bool SetCircleInstanceData(RendererData data, int i, Vector3 scale, float depth, Color color, Matrix4x4 scaledLookAtCamera)
        {
            var renderer = data.Renderer;
            var center = renderer.bounds.center;
            var viewPoint = shapeCamera.WorldToViewportPoint(center);

            if (!CameraUtils.IsBetweenNearAndFar(circleCamera, viewPoint.z))
            {
                return false;
            }
            //Debug.Log($"{GetType().Name}.SetCircleInstanceData: {viewPoint}");
            viewPoint.z = circleCamera.nearClipPlane + depth;
            var worldPoint = circleCamera.ViewportToWorldPoint(viewPoint);
            var objectToWorld = scaledLookAtCamera;
            objectToWorld.SetColumn(3, worldPoint);
            objectToWorld.m33 = 1;

            var clipPoint = new Vector3()
            {
                x = (viewPoint.x - 0.5f) * 2f,
                y = (viewPoint.y - 0.5f) * 2f,
                z = depth
            };
            //Debug.Log($"{GetType().Name}.{i} | {data.CameraDistanceSqr} | {clipPoint.z}");
            circleColor[i] = color;
            circleObjectToWorld[i] = objectToWorld;
            circleInstanceData[i] = new InstanceData()
            {
                clipPosition = clipPoint,
                color = color,
                scale = scale
            };
            return true;
        }

        public void OnLateUpdate(/*OutlineCamera outlineCamera*/)
        {
            SortOutlineObjects();
            SetSortedObjectsColorWithDepth();
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
