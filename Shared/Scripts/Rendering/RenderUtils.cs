using System;
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

//        public static void ExecuteCommandBuffer(ScriptableRenderContext context, Action<CommandBuffer> renderAction, bool submitContext = true)
//        {
//#if UNITY_PIPELINE_CORE
//            var cmd = CommandBufferPool.Get();
//#else
//            var cmd = cmdBuffer = new CommandBuffer();
//#endif
//            renderAction(cmd);

//            context.ExecuteCommandBuffer(cmd);

//            if (submitContext)
//            {
//                context.Submit();
//            }
//#if UNITY_PIPELINE_CORE
//            CommandBufferPool.Release(cmd);
//#else
//            cmd.Release();
//#endif
//        }
    }
}
