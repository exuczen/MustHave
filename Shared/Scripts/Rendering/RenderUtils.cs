using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_PIPELINE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace MustHave
{
    public struct RenderUtils
    {
#if UNITY_PIPELINE_URP
        public static bool UniversalRenderPipelineInstalled => true;
#else
        public static bool UniversalRenderPipelineInstalled => false;
#endif

        public static RenderPipelineType GetRenderPipelineType()
        {
            RenderPipelineType pipelineType;
            var pipeline = QualitySettings.renderPipeline;

            if (!pipeline)
            {
                pipeline = GraphicsSettings.defaultRenderPipeline;
            }
            if (pipeline)
            {
#if UNITY_PIPELINE_URP
                if (pipeline is UniversalRenderPipelineAsset)
                {
                    pipelineType = RenderPipelineType.URP;
                }
                else
#endif
#if UNITY_PIPELINE_HDRP
                if (pipeline is HDRenderPipelineAsset)
                {
                    pipelineType = RenderPipelineType.HDRP;
                }
                else
#endif
                {
                    pipelineType = RenderPipelineType.CustomSRP;
                }
                //Debug.Log($"RenderPipelineAsset: {pipeline.GetType().Name} | {pipelineType}");
            }
            else
            {
                pipelineType = RenderPipelineType.Default;
                //Debug.Log($"Built-in Render Pipeline | {pipelineType}");
            }
            return pipelineType;
        }

        public static void Render(ScriptableRenderContext context, Camera camera, RenderPipelineType pipelineType)
        {
            switch (pipelineType)
            {
                case RenderPipelineType.Default:
                    throw new System.InvalidOperationException();
#if UNITY_PIPELINE_URP
                case RenderPipelineType.URP:
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0618 // Type or member is obsolete
                    UniversalRenderPipeline.RenderSingleCamera(context, camera);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore IDE0079 // Remove unnecessary suppression
                    break;
#endif
#if UNITY_PIPELINE_HDRP
                case RenderPipelineType.HDRP:
                    throw new System.NotImplementedException();
#endif
                case RenderPipelineType.CustomSRP:
                default:
                    throw new System.InvalidOperationException();
            }
        }
    }
}
