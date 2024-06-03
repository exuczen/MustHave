#if UNITY_EDITOR
//#define USE_EDITOR_SCENE_EVENTS
#endif

using MustHave.Utils;
using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_PIPELINE_HDRP
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
        public RenderPipelineType PipelineType { get; set; } = RenderPipelineType.Default;

        [SerializeField]
        protected ComputeShader shader = null;
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

        protected int mainKernelID = -1;
        protected bool initialized = false;

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
            if (!cameraChangeListener)
            {
                cameraChangeListener = gameObject.AddComponent<CameraChangeListener>();
            }
            mainKernelID = shader.FindKernel(MainKernelName);

            PipelineType = RenderUtils.GetRenderPipelineType();

            ReleaseTextures();
            CreateTextures();

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
            texture = new RenderTexture(textureSize.x, textureSize.y, 0)
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

            if (shader)
            {
                shader.GetKernelThreadGroupSizes(mainKernelID, out uint x, out uint y, out _);
                threadGroups.x = Utils.ShaderUtils.GetThreadGroupsCount(x, textureSize.x);
                threadGroups.y = Utils.ShaderUtils.GetThreadGroupsCount(y, textureSize.y);
            }
            CreateTexture(ref outputTexture);
            CreateTexture(ref sourceTexture);

            shader.SetTexture(mainKernelID, SourceTextureID, sourceTexture);
            shader.SetTexture(mainKernelID, OutputTextureID, outputTexture);

            sourceRenderTarget = new RenderTargetIdentifier(sourceTexture);
            outputRenderTarget = new RenderTargetIdentifier(outputTexture);

#if UNITY_PIPELINE_URP
            if (renderPass == null)
            {
                renderPass = new ComputeShaderRenderPass(renderPassSettings, this);
            }
            else
            {
                renderPass.Init(renderPassSettings, this);
            }
#endif
        }

        protected virtual void OnInit() { }

        protected virtual void OnCameraPropertyChange() { }

        protected virtual void OnScreenSizeChange() { }

        protected virtual void SetupOnRenderImage() { }

        protected virtual void SetupOnRenderImage(CommandBuffer cmd) { }

        protected virtual void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
#if UNITY_PIPELINE_URP
            if (camera == thisCamera)
            {
                var cameraData = camera.GetUniversalAdditionalCameraData();
                if (cameraData)
                {
                    //Debug.Log($"{GetType().Name}.OnBeginCameraRendering: {cameraData.scriptableRenderer}");
                    cameraData.scriptableRenderer?.EnqueuePass(renderPass);
                }
            }
#endif
        }

        protected virtual void OnEndCameraRendering(ScriptableRenderContext context, Camera camera) { }

        protected virtual void DispatchShader()
        {
            shader.Dispatch(mainKernelID, threadGroups.x, threadGroups.y, 1);
        }

        protected virtual void DispatchShader(CommandBuffer cmd)
        {
            cmd.DispatchCompute(shader, mainKernelID, threadGroups.x, threadGroups.y, 1);
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

                //UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
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
            }
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

                //UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
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
            }
            ReleaseTextures();
            initialized = false;
        }

        protected virtual void OnDestroy()
        {
            ReleaseTextures();
            initialized = false;
        }

        public bool OnExecuteRenderPass(ComputeShaderRenderPass pass, CommandBuffer cmd, RenderTargetIdentifier colorRenderTarget)
        {
            if (initialized && shader && !SkipDispatch)
            {
                CheckResolution(out _);
                SetupOnRenderImage(cmd);

                pass.Blit(cmd, colorRenderTarget, sourceRenderTarget);

                DispatchShader(cmd);

                pass.Blit(cmd, outputRenderTarget, colorRenderTarget);

                return true;
            }
            else
            {
                return false;
            }
        }

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (initialized && shader && !SkipDispatch)
            {
                CheckResolution(out _);
                SetupOnRenderImage();

                Graphics.Blit(source, sourceTexture);

                DispatchShader();

                Graphics.Blit(outputTexture, destination);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        private void CheckResolution(out bool changed)
        {
            changed = textureSize.x != thisCamera.pixelWidth || textureSize.y != thisCamera.pixelHeight;

            if (changed)
            {
                ReleaseTextures();
                CreateTextures();
                OnScreenSizeChange();
            }
        }

#if UNITY_EDITOR
#if USE_EDITOR_SCENE_EVENTS
        protected virtual void OnActiveSceneChangedInEditMode(Scene prevScene, Scene scene)
        {
            Debug.Log($"{GetType().Name}.OnActiveSceneChangedInEditMode");
        }

        protected virtual void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            Debug.Log($"{GetType().Name}.OnSceneOpened");
        }
#endif
        private void OnAllAssetsPostprocessed()
        {
            ReInit();
        }
#endif
    }
}
