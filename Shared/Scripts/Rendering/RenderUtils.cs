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

        public static RenderPipelineAsset GetRenderPipelineAsset()
        {
            var pipeline = GraphicsSettings.currentRenderPipeline;
            if (!pipeline)
            {
                pipeline = QualitySettings.renderPipeline;
                if (!pipeline)
                {
                    pipeline = GraphicsSettings.defaultRenderPipeline;
                }
            }
            return pipeline;
        }

        public static RenderPipelineType GetRenderPipelineType(RenderPipelineAsset pipeline)
        {
            RenderPipelineType pipelineType;
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

        public static RenderPipelineType GetRenderPipelineType(RenderPipeline pipeline)
        {
            RenderPipelineType pipelineType;
            if (pipeline != null)
            {
#if UNITY_PIPELINE_URP
                if (pipeline is UniversalRenderPipeline)
                {
                    pipelineType = RenderPipelineType.URP;
                }
                else
#endif
#if UNITY_PIPELINE_HDRP
                if (pipeline is HDRenderPipeline)
                {
                    pipelineType = RenderPipelineType.HDRP;
                }
                else
#endif
                {
                    pipelineType = RenderPipelineType.CustomSRP;
                }
                //Debug.Log($"RenderPipeline: {pipeline.GetType().Name} | {pipelineType}");
            }
            else
            {
                pipelineType = RenderPipelineType.Default;
                //Debug.Log($"Built-in Render Pipeline | {pipelineType}");
            }
            return pipelineType;
        }

        public static RenderPipelineType GetRenderPipelineType()
        {
            return GetRenderPipelineType(GetRenderPipelineAsset());
        }
    }
}
