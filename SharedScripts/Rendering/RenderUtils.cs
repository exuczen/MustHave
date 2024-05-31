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
                Debug.Log($"RenderPipelineAsset: {pipeline.GetType()} | {pipelineType}");
            }
            else
            {
                pipelineType = RenderPipelineType.Default;
                Debug.Log($"Built-in Render Pipeline | {pipelineType}");
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
                    UniversalRenderPipeline.RenderSingleCamera(context, camera);
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
