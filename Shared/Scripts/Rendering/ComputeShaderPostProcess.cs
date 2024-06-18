#if UNITY_EDITOR
//#define USE_EDITOR_SCENE_EVENTS
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_PIPELINE_HDRP
using static UnityEngine.Rendering.HighDefinition.RenderPipelineSettings;
using UnityEngine.Rendering.HighDefinition;
#endif
#if USE_EDITOR_SCENE_EVENTS
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
#endif

namespace MustHave
{
    [RequireComponent(typeof(Camera))]
    public class ComputeShaderPostProcess : MonoBehaviour
    {
        protected static readonly int SourceTextureID = Shader.PropertyToID("Source");
        protected static readonly int OutputTextureID = Shader.PropertyToID("Output");

        public Camera Camera => thisCamera;
        public RenderTexture OutputTexture => outputTexture;
        public RenderTexture SourceTexture => sourceTexture;
        public RenderPipelineType PipelineType => pipelineType;

        protected bool HasCommandBuffer => cmdBuffer != null;

        private bool CanExecute => initialized && shader && !SkipDispatch;
        private bool IsCameraPixelSizeValid =>
            thisCamera.pixelWidth > 0 && thisCamera.pixelWidth <= SystemInfo.maxTextureSize &&
            thisCamera.pixelHeight > 0 && thisCamera.pixelHeight <= SystemInfo.maxTextureSize &&
            thisCamera.aspect <= 75f;

        [SerializeField]
        protected ComputeShader shader = null;
        [SerializeField, HideInInspector]
        protected RenderPipelineType pipelineType = RenderPipelineType.Default;
#if UNITY_PIPELINE_URP
        [SerializeField]
        protected RenderPassSettings renderPassSettings = new();
        protected ComputeShaderRenderPass renderPass = null;
#endif
        protected virtual string MainKernelName => "CSMain";
        protected virtual bool SkipDispatch => false;

        protected Vector2Int textureSize = Vector2Int.zero;
        /// <summary>
        /// Main kernel's thread group counts
        /// </summary>
        protected Vector2Int threadGroups = Vector2Int.zero;

        protected Camera thisCamera = null;
        protected CameraChangeListener cameraChangeListener = null;

        protected RenderTexture outputTexture = null;
        protected RenderTexture sourceTexture = null;
        protected RenderTargetIdentifier sourceRenderTarget;
        protected RenderTargetIdentifier outputRenderTarget;

        protected CommandBuffer cmdBuffer = null;

        protected int mainKernelID = -1;
        protected bool initialized = false;

        protected void Awake()
        {
            var renderPipeline = RenderUtils.GetRenderPipelineAsset();

            var prevPipelineType = pipelineType;
            pipelineType = RenderUtils.GetRenderPipelineType(renderPipeline);

#if UNITY_PIPELINE_HDRP
            if (pipelineType == RenderPipelineType.HDRP)
            {
                var hdrp = renderPipeline as HDRenderPipelineAsset;
                var colorBufferFormat = hdrp.currentPlatformRenderPipelineSettings.colorBufferFormat;
                if (colorBufferFormat != ColorBufferFormat.R16G16B16A16)
                {
                    hdrp.ModifySettings((ref RenderPipelineSettings settings) => {
                        settings.colorBufferFormat = ColorBufferFormat.R16G16B16A16;
                    });
                }
            }
#endif
            OnAwake(prevPipelineType != pipelineType);
        }

        protected void Init()
        {
            if (initialized)
            {
                return;
            }
            if (!SystemInfo.supportsComputeShaders)
            {
                Debug.LogError($"{GetType().Name}.Init: It seems your target Hardware does not support Compute Shaders.");
                return;
            }
            if (!shader)
            {
                Debug.LogError($"{GetType().Name}.Init: No shader.");
                return;
            }
            thisCamera = GetComponent<Camera>();
            cameraChangeListener = GetComponent<CameraChangeListener>();

            if (!thisCamera)
            {
                Debug.LogError($"{GetType().Name}.Init: Object has no Camera.");
                return;
            }
            if (!IsCameraPixelSizeValid)
            {
                Debug.LogError($"{GetType().Name}.Init: Invalid camera pixel size: ({thisCamera.pixelWidth}, {thisCamera.pixelHeight}) OR aspect: {thisCamera.aspect}");
                return;
            }
            if (!cameraChangeListener)
            {
                cameraChangeListener = gameObject.AddComponent<CameraChangeListener>();
            }
            mainKernelID = shader.FindKernel(MainKernelName);

            ReleaseTextures();
            CreateTextures();

            pipelineType = RenderUtils.GetRenderPipelineType();

#if UNITY_PIPELINE_URP
            if (PipelineType == RenderPipelineType.URP)
            {
                thisCamera.AddUniversalAdditionalCameraData();

                if (renderPass == null)
                {
                    renderPass = new ComputeShaderRenderPass(renderPassSettings, this);
                }
                else
                {
                    renderPass.Setup(renderPassSettings, this);
                }
            }
#endif
            OnInit();

            initialized = true;
        }

        protected void ReInit()
        {
            initialized = false;
            Init();
        }

        protected void ReleaseTexture(ref RenderTexture texture)
        {
            if (null != texture)
            {
                texture.Release();
                texture = null;
            }
        }

        protected void CreateTexture(ref RenderTexture texture)
        {
            texture = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.ARGBFloat)
            {
                enableRandomWrite = true
            };
            texture.Create();
        }

        protected virtual void ReleaseTextures()
        {
            ReleaseTexture(ref outputTexture);
            ReleaseTexture(ref sourceTexture);
        }

        protected virtual void CreateTextures()
        {
            textureSize.x = thisCamera.pixelWidth;
            textureSize.y = thisCamera.pixelHeight;

            shader.GetKernelThreadGroupSizes(mainKernelID, out uint x, out uint y, out _);
            threadGroups.x = ShaderUtils.GetThreadGroupsCount(x, textureSize.x);
            threadGroups.y = ShaderUtils.GetThreadGroupsCount(y, textureSize.y);

            CreateTexture(ref outputTexture);
            CreateTexture(ref sourceTexture);

            sourceRenderTarget = new RenderTargetIdentifier(sourceTexture);
            outputRenderTarget = new RenderTargetIdentifier(outputTexture);

            //if (HasCommandBuffer)
            //{
            //    cmdBuffer.SetComputeTextureParam(shader, mainKernelID, SourceTextureID, sourceRenderTarget);
            //    cmdBuffer.SetComputeTextureParam(shader, mainKernelID, OutputTextureID, outputRenderTarget);
            //}
            shader.SetTexture(mainKernelID, SourceTextureID, sourceTexture);
            shader.SetTexture(mainKernelID, OutputTextureID, outputTexture);
        }

        protected virtual void OnAwake(bool pipelineChanged) { }

        protected virtual void OnInit() { }

        protected virtual void OnCameraPropertyChange() { }

        protected virtual void OnScreenSizeChange() { }

        protected virtual void OnLateUpdate() { }

        protected virtual void SetupOnRenderImage() { }

        protected virtual void SetupOnExecute(CommandBuffer cmd) { }

        protected virtual void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera) { }

        protected virtual void OnEndCameraRendering(ScriptableRenderContext context, Camera camera) { }

        protected virtual void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
#if UNITY_PIPELINE_URP
            if (PipelineType != RenderPipelineType.URP)
            {
                return;
            }
            var cameraData = thisCamera.GetUniversalAdditionalCameraData();
            if (cameraData)
            {
                //Debug.Log($"{GetType().Name}.OnBeginContextRendering: {cameraData.scriptableRenderer}");
                renderPass.Setup(renderPassSettings, this);
                cameraData.scriptableRenderer?.EnqueuePass(renderPass);
            }
#endif
        }

        protected virtual void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
#if UNITY_EDITOR
            if (SceneUtils.GetCurrentSceneViewCamera())
            {
                return;
            }
#endif
            if (PipelineType != RenderPipelineType.CustomSRP &&
                PipelineType != RenderPipelineType.HDRP)
            {
                return;
            }
            var cameraTarget = BuiltinRenderTextureType.CameraTarget;

            OnExecuteRenderPass(context,
                cmd => {
                    cmd.Blit(cameraTarget, sourceRenderTarget);
                    cmd.Blit(sourceRenderTarget, cameraTarget);
                    cmd.Blit(cameraTarget, sourceRenderTarget);
                },
                cmd => cmd.Blit(outputRenderTarget, cameraTarget)
            );
        }

        protected virtual void DispatchShader()
        {
            shader.Dispatch(mainKernelID, threadGroups.x, threadGroups.y, 1);
        }

        protected virtual void DispatchShader(CommandBuffer cmd)
        {
            cmd.DispatchCompute(shader, mainKernelID, threadGroups.x, threadGroups.y, 1);
        }

        protected virtual void LateUpdate()
        {
            CheckResolution(out _);
            OnLateUpdate();
        }

        protected virtual void OnValidate()
        {
            if (initialized)
            {
                OnInit();
            }
        }

        protected virtual void OnEnable()
        {
            Init();
            if (!initialized)
            {
                return;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
#if USE_EDITOR_SCENE_EVENTS
                EditorSceneManager.sceneOpened -= OnSceneOpened;
                EditorSceneManager.sceneOpened += OnSceneOpened;
                EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChangedInEditMode;
                EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChangedInEditMode;
#endif
                UnityAssetPostprocessor.AllAssetsPostprocessed -= OnAllAssetsPostprocessed;
                UnityAssetPostprocessor.AllAssetsPostprocessed += OnAllAssetsPostprocessed;

                UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }
#endif
            if (cameraChangeListener)
            {
                cameraChangeListener.PropertyChanged -= OnCameraPropertyChange;
                cameraChangeListener.PropertyChanged += OnCameraPropertyChange;
            }
            if (PipelineType != RenderPipelineType.Default)
            {
                RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
                RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
                RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
                RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
                RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
                RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
                RenderPipelineManager.endContextRendering -= OnEndContextRendering;
                RenderPipelineManager.endContextRendering += OnEndContextRendering;
            }
            //#if UNITY_PIPELINE_HDRP
            //            if (PipelineType == RenderPipelineType.HDRP)
            //            {
            //                hdCameraData.customRender -= OnCustomRender;
            //                hdCameraData.customRender += OnCustomRender;
            //            }
            //#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
#if USE_EDITOR_SCENE_EVENTS
                EditorSceneManager.sceneOpened -= OnSceneOpened;
                EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChangedInEditMode;
#endif
                UnityAssetPostprocessor.AllAssetsPostprocessed -= OnAllAssetsPostprocessed;

                UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            }
#endif
            if (cameraChangeListener)
            {
                cameraChangeListener.PropertyChanged -= OnCameraPropertyChange;
            }
            if (PipelineType != RenderPipelineType.Default)
            {
                RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
                RenderPipelineManager.beginCameraRendering -= OnEndCameraRendering;
                RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
                RenderPipelineManager.endContextRendering -= OnEndContextRendering;
            }
            //#if UNITY_PIPELINE_HDRP
            //            if (PipelineType == RenderPipelineType.HDRP)
            //            {
            //                hdCameraData.customRender -= OnCustomRender;
            //            }
            //#endif
            OnDisableOrDestroy();
        }

        protected virtual void OnDestroy()
        {
            OnDisableOrDestroy();
        }

#if UNITY_EDITOR
        protected virtual void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange) { }
#endif

        //#if UNITY_PIPELINE_HDRP
        //        protected virtual void OnCustomRender(ScriptableRenderContext context, HDCamera camera) { }
        //#endif

#if UNITY_PIPELINE_URP
        public bool OnExecuteRenderPass(ComputeShaderRenderPass pass, ScriptableRenderContext context, RenderTargetIdentifier colorRenderTarget)
        {
            return OnExecuteRenderPass(context,
                cmd => pass.Blit(cmd, colorRenderTarget, sourceRenderTarget),
                cmd => pass.Blit(cmd, outputRenderTarget, colorRenderTarget)
            );
        }
#endif
        protected bool OnExecuteRenderPass(ScriptableRenderContext context, RenderTargetIdentifier colorRenderTarget)
        {
            return OnExecuteRenderPass(context,
                cmd => cmd.Blit(colorRenderTarget, sourceRenderTarget),
                cmd => cmd.Blit(outputRenderTarget, colorRenderTarget)
            );
        }

        protected bool OnExecuteRenderPass(ScriptableRenderContext context, Action<CommandBuffer> blitSource, Action<CommandBuffer> blitOutput)
        {
            if (CanExecute)
            {
                context.ExecuteCommandBuffer(cmd => {
                    cmdBuffer = cmd;

                    blitSource(cmd);

                    SetupOnExecute(cmd);
                    DispatchShader(cmd);

                    blitOutput(cmd);

                    cmdBuffer = null;
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CanExecute)
            {
                Graphics.Blit(source, sourceTexture);

                SetupOnRenderImage();
                DispatchShader();

                Graphics.Blit(outputTexture, destination);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        private void OnDisableOrDestroy()
        {
            ReleaseTextures();
            initialized = false;
            cmdBuffer = null; // It is released by ScriptableRenderContextExtensionMethods.ExecuteCommandBuffer
        }

        private void CheckResolution(out bool changed)
        {
            changed = textureSize.x != thisCamera.pixelWidth || textureSize.y != thisCamera.pixelHeight;

            if (changed)
            {
                if (IsCameraPixelSizeValid)
                {
                    ReleaseTextures();
                    CreateTextures();
                    OnScreenSizeChange();
                }
                else
                {
                    Debug.LogError($"{GetType().Name}.CheckResolution: Invalid camera pixel size: ({thisCamera.pixelWidth}, {thisCamera.scaledPixelHeight}) OR aspect: {thisCamera.aspect}");
                }
            }
        }

#if UNITY_EDITOR
#if USE_EDITOR_SCENE_EVENTS
        protected virtual void OnActiveSceneChangedInEditMode(Scene prevScene, Scene scene)
        {
            Debug.Log($"{GetType().Name}.OnActiveSceneChangedInEditMode: {scene.name}");
        }

        protected virtual void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            Debug.Log($"{GetType().Name}.OnSceneOpened: {scene.name} | {mode}");
        }
#endif
        protected virtual void OnAllAssetsPostprocessed()
        {
            ReInit();
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        }
#endif
    }
}
