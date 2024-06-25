using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace MustHave
{
    public interface IOutlineCameraSingleton : IMonoSingleton<OutlineCamera> { }

    [ExecuteInEditMode]
    public class OutlineCamera : ComputeShaderPostProcess, IOutlineCameraSingleton
    {
        public const int LineMaxThickness = 100;

        public OutlineObjectCamera ObjectCamera => objectCamera;

        public int LineThickness
        {
            get => lineThickness;
            set
            {
                lineThickness = Mathf.Clamp(value, 1, LineMaxThickness);
                ScaledLineThickness = (int)(0.5f + thisCamera.pixelHeight * value / (7f * LineMaxThickness));
                ScaledLineThickness = Mathf.Max(1 + ShaderSettings.SmoothRadius, ScaledLineThickness);
            }
        }
        public int ScaledLineThickness { get; private set; }

        public OutlineShaderSettings ShaderSettings => objectCamera ? objectCamera.ShaderSettings : null;
        public bool ShaderSettingsExpanded { get => shaderSettingsExpanded; set => shaderSettingsExpanded = value; }

        protected override bool SkipDispatch => objectCamera.ObjectsCount == 0;

        private readonly struct ShaderData
        {
            public static readonly int ShapeTexID = Shader.PropertyToID("ShapeTexture");
            public static readonly int ShapeTexSizeID = Shader.PropertyToID("ShapeTexSize");
            public static readonly int ShapeTexOffsetID = Shader.PropertyToID("ShapeTexOffset");
            public static readonly int CircleTexID = Shader.PropertyToID("CircleTexture");
            public static readonly int LineThicknessID = Shader.PropertyToID("LineThickness");
            public static readonly int SmoothWeightsID = Shader.PropertyToID("SmoothWeightsBuffer");
            public static readonly int SmoothRadiusID = Shader.PropertyToID("SmoothRadius");
            public static readonly int SmoothPowerID = Shader.PropertyToID("SmoothPower");
            public static readonly int OutlineTextureArrayID = Shader.PropertyToID("OutlineTextureArray");
        }

        [SerializeField]
        private OutlineObjectCamera objectCamera = null;

        [SerializeField, Range(1, LineMaxThickness)]
        private int lineThickness = 5;

        [SerializeField, HideInInspector]
        private bool shaderSettingsExpanded = true;

        private Texture2DArray outlineTextureArray = null;
        private RenderTexture outlineTextures = null;

        private ComputeBuffer smoothWeightsBuffer = null;

        private float[] smoothWeights = null;

        private int mergeOutlinesKernelID = -1;
        private int mergeWithSourceKernelID = -1;

        public void SetSmoothWeights(bool log = false)
        {
            if (!ShaderSettings)
            {
                return;
            }
            // Fill weights array
            {
                int smoothRadius = ShaderSettings.SmoothRadius;
                int smoothLength = (smoothRadius << 1) + 1;
                smoothWeights = new float[smoothLength * smoothLength];

                void setWeight(int dx, int dy, float weight)
                {
                    int xOffset = smoothRadius + dx;
                    int yOffset = smoothRadius + dy;
                    int index = xOffset + yOffset * smoothLength;
                    smoothWeights[index] = weight;
                }
                float getWeight(int x, int y) => smoothWeights[x + y * smoothLength];

                float maxRadius = Mathf.Max(1f, Mathf.Sqrt(2) * smoothRadius);
                float minWeight = 1f / smoothLength;
                int power = ShaderSettings.SmoothWeightsPower;

                for (int dy = 0; dy <= smoothRadius; dy++)
                {
                    for (int dx = 0; dx <= smoothRadius; dx++)
                    {
                        float r = Mathf.Sqrt(dx * dx + dy * dy);
                        float transition = Maths.GetTransitionAsymNormalized(r, maxRadius, 0.5f, power, power);
                        float weight = 1f - transition + minWeight;

                        setWeight(dx, dy, weight);
                        setWeight(-dx, dy, weight);
                        setWeight(dx, -dy, weight);
                        setWeight(-dx, -dy, weight);
                    }
                }
                if (log)
                {
                    string weightsString = "";
                    for (int y = 0; y < smoothLength; y++)
                    {
                        for (int x = 0; x < smoothLength; x++)
                        {
                            weightsString = $"{weightsString} {getWeight(x, y):f2}";
                        }
                        weightsString = $"{weightsString}\n";
                    }
                    Debug.Log($"{GetType().Name}.SetSmoothWeghts:\n{weightsString}");
                }
            }
            // Set shader data
            {
                if (smoothWeightsBuffer == null || smoothWeightsBuffer.count != smoothWeights.Length)
                {
                    smoothWeightsBuffer?.Release();
                    smoothWeightsBuffer = new ComputeBuffer(smoothWeights.Length, sizeof(float));
                }
                smoothWeightsBuffer.SetData(smoothWeights);
                shader.SetBuffer(mergeWithSourceKernelID, ShaderData.SmoothWeightsID, smoothWeightsBuffer);

                shader.SetInt(ShaderData.SmoothRadiusID, ShaderSettings.SmoothRadius);
                shader.SetInt(ShaderData.SmoothPowerID, ShaderSettings.SmoothPower);
            }
            // Update line thickness
            {
                LineThickness = lineThickness;
            }
        }

        protected override void OnAwake(bool pipelineChanged)
        {
            IOutlineCameraSingleton.SetInstanceOnAwake(this, out var gameObject);
#if UNITY_EDITOR
            if (this == IOutlineCameraSingleton.Instance)
            {
                if (!Application.isPlaying && pipelineChanged)
                {
                    AssetUtils.ModifyPrefab<OutlineObjectCamera>(objectCamera => {
                        objectCamera.SetupCameraAdditionalData(PipelineType);
                    }, OutlineObjectCamera.PrefabPath);

                    EditorApplicationUtils.AddSingleActionOnEditorUpdate(() => enabled = true);

                    enabled = false;
                }
            }
            else if (gameObject.TryGetComponent<Camera>(out var camera))
            {
                camera.enabled = false;
            }
#endif
        }

        protected override void OnEnable()
        {
            if (IOutlineCameraSingleton.Instance != this)
            {
                enabled = false;
                return;
            }
            if (!objectCamera)
            {
                objectCamera = GetComponentInChildren<OutlineObjectCamera>();

                if (!objectCamera)
                {
                    var objectCameraPrefab = Resources.Load<OutlineObjectCamera>(OutlineObjectCamera.PrefabName);
                    if (objectCameraPrefab)
                    {
#if UNITY_EDITOR
                        objectCamera = PrefabUtility.InstantiatePrefab(objectCameraPrefab, transform) as OutlineObjectCamera;
#else
                        objectCamera = Instantiate(objectCameraPrefab, transform);
#endif
                        objectCamera.name = OutlineObjectCamera.PrefabName;
                    }
                    else
                    {
                        Debug.LogError($"{GetType().Name}.OnEnable: No {OutlineObjectCamera.PrefabName}.prefab in Resources folder.");
                        enabled = false;
                        return;
                    }
                }
                objectCamera.transform.Reset();

                shader = objectCamera.ComputeShader;
            }
            base.OnEnable();

            enabled = initialized;

#if UNITY_EDITOR
            if (PipelineType == RenderPipelineType.HDRP)
            {
                // Hack for internal UnityEngine.Rendering.HighDefinition MissingReferenceException:
                // The object of type 'HDAdditionalCameraData' has been destroyed but you are still trying to access it.
                // UpdateDebugCameraName is added to UnityEditor.EditorApplication.hierarchyChanged event and called
                // after the runtime's game object has been destroyed on exiting play mode and entering edit mode.

                objectCamera.SetGameObjectActive(initialized && Application.isPlaying);

                if (initialized && !Application.isPlaying)
                {
                    EditorApplicationUtils.AddSingleActionOnEditorUpdate(() => {
                        if (objectCamera)
                        {
                            objectCamera.SetGameObjectActive(true);
                        }
                    });
                }
            }
            else
#endif
            {
                objectCamera.SetGameObjectActive(initialized);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (objectCamera)
            {
                objectCamera.SetGameObjectActive(false);
            }
            smoothWeightsBuffer?.Release();
            smoothWeightsBuffer = null;
        }

        protected override void OnValidate()
        {
            if (initialized)
            {
                SetupOnCameraChange(false);
            }
        }

        protected override void OnInit()
        {
            SetupOnCameraChange();

            if (SceneUtils.IsActiveSceneLoadedAndValid())
            {
                ShaderSettings.SetDebugModeOnInit();
            }
            SetSmoothWeights(false);
        }

        protected override void FindKernels()
        {
            base.FindKernels();

            mergeOutlinesKernelID = shader.FindKernel("MergeOutlines");
            mergeWithSourceKernelID = shader.FindKernel("MergeWithSource");
        }

        protected override void CreateTextures()
        {
            base.CreateTextures();

            outlineTextures = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                dimension = TextureDimension.Tex2DArray,
                volumeDepth = 2,
            };
            outlineTextures.Create();

            outlineTextureArray = new Texture2DArray(textureSize.x, textureSize.y, outlineTextures.volumeDepth, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(outlineTextures, outlineTextureArray);
            outlineTextureArray.Apply();

            shader.SetTexture(mainKernelID, ShaderData.OutlineTextureArrayID, outlineTextures);
            shader.SetTexture(mergeOutlinesKernelID, ShaderData.OutlineTextureArrayID, outlineTextures);
            shader.SetTexture(mergeOutlinesKernelID, OutputTextureID, outputTexture);

            var shapeTexSize = GetShapeTexSize(out var shapeTexOffset);

            objectCamera.CreateRuntimeAssets(shapeTexSize);

            shader.SetTexture(mainKernelID, ShaderData.ShapeTexID, objectCamera.ShapeTexture);
            shader.SetTexture(mainKernelID, ShaderData.CircleTexID, objectCamera.CircleTexture);

            shader.SetInts(ShaderData.ShapeTexSizeID, shapeTexSize.x, shapeTexSize.y);
            shader.SetInts(ShaderData.ShapeTexOffsetID, shapeTexOffset.x, shapeTexOffset.y);

            shader.SetTexture(mergeWithSourceKernelID, ShaderData.ShapeTexID, objectCamera.ShapeTexture);
            shader.SetTexture(mergeWithSourceKernelID, ShaderData.CircleTexID, objectCamera.CircleTexture);
            shader.SetTexture(mergeWithSourceKernelID, SourceTextureID, sourceTexture);
            shader.SetTexture(mergeWithSourceKernelID, OutputTextureID, outputTexture);
        }

        protected override void ReleaseTextures()
        {
            base.ReleaseTextures();

            ReleaseTexture(ref outlineTextures);

            if (objectCamera)
            {
                objectCamera.DestroyRuntimeAssets();
            }
        }

        protected override void DispatchShader()
        {
            base.DispatchShader();

            shader.Dispatch(mergeOutlinesKernelID, threadGroups.x, threadGroups.y, 1);

            shader.Dispatch(mergeWithSourceKernelID, threadGroups.x, threadGroups.y, 1);
        }

        protected override void DispatchShader(CommandBuffer cmd)
        {
            base.DispatchShader(cmd);

            cmd.DispatchCompute(shader, mergeOutlinesKernelID, threadGroups.x, threadGroups.y, 1);

            cmd.DispatchCompute(shader, mergeWithSourceKernelID, threadGroups.x, threadGroups.y, 1);
        }

        protected override void OnCameraPropertyChange()
        {
            SetupOnCameraChange();
        }

        protected override void OnScreenSizeChange()
        {
            SetupOnCameraChange();
        }

        protected override void SetupOnRenderImage()
        {
            if (PipelineType != RenderPipelineType.Default)
            {
                throw new InvalidOperationException($"RenderPipelineType: {PipelineType}");
            }
            objectCamera.RenderShapes();
        }

        protected override void OnLateUpdate()
        {
            objectCamera.OnLateUpdate();

            if (PipelineType != RenderPipelineType.HDRP)
            {
                objectCamera.RenderCircles(ScaledLineThickness);
            }
            shader.SetInt(ShaderData.LineThicknessID, ScaledLineThickness);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //if (ShaderSettings)
            //{
            //    ShaderSettings.SetDebugModeFromInit();
            //}
            IOutlineCameraSingleton.ClearInstanceOnDestroy(this);

            ObjectUtils.DestroyGameObject(ref objectCamera);
            ObjectUtils.DestroyComponent(ref cameraChangeListener);
        }

        private void SetupOnCameraChange(bool copySettings = true)
        {
            LineThickness = lineThickness;

            if (objectCamera)
            {
                objectCamera.Setup(this, copySettings);
            }
        }

        private Vector2Int GetShapeTexSize(out Vector2Int shapeTexOffset)
        {
            Vector2Int extendedSize = default;
            Vector2Int texOffset = default;
            var texSize = textureSize;
            int offset = LineMaxThickness;

            if (texSize.x > texSize.y)
            {
                extendedSize.y = texSize.y + 2 * offset;
                extendedSize.x = extendedSize.y * texSize.x / texSize.y;

                texOffset.y = offset;
                texOffset.x = (int)(1f * texOffset.y * texSize.x / texSize.y + 0.5f);
            }
            else
            {
                extendedSize.x = texSize.x + 2 * offset;
                extendedSize.y = extendedSize.x * texSize.y / texSize.x;

                texOffset.x = offset;
                texOffset.y = (int)(1f * texOffset.x * texSize.y / texSize.x + 0.5f);
            }
            shapeTexOffset = texOffset;

            return extendedSize;
        }

        protected override void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (objectCamera)
            {
                if (camera == objectCamera.ShapeCamera)
                {
                    objectCamera.OnBeginRenderingShapes();
                }
                else
                {
                    base.OnBeginCameraRendering(context, camera);
                }
            }
        }

        protected override void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (objectCamera)
            {
                if (camera == objectCamera.ShapeCamera)
                {
                    objectCamera.OnEndRenderingShapes();
                }
                else if (camera == objectCamera.CircleCamera && PipelineType == RenderPipelineType.HDRP)
                {
                    context.ExecuteCommandBuffer(cmd => {
                        objectCamera.RenderCircles(ScaledLineThickness, cmd);
                    });
                }
                else
                {
                    base.OnEndCameraRendering(context, camera);
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode && ShaderSettings)
            {
                ShaderSettings.SetDebugModeFromInit();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
#endif
    }
}
