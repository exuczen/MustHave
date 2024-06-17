using System;
using UnityEngine.Rendering;

namespace MustHave
{
    public static class ScriptableRenderContextExtensionMethods
    {
        public static void ExecuteCommandBuffer(this ScriptableRenderContext context, Action<CommandBuffer> renderAction, bool submit = true)
        {
#if UNITY_PIPELINE_CORE
            var cmd = CommandBufferPool.Get();
#else
            var cmd = new CommandBuffer();
#endif
            renderAction(cmd);

            context.ExecuteCommandBuffer(cmd);

            if (submit)
            {
                context.Submit();
            }
#if UNITY_PIPELINE_CORE
            CommandBufferPool.Release(cmd);
#else
            cmd.Release();
#endif
        }
    }
}
