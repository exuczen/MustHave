#if UNITY_PIPELINE_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MustHave
{
    public class ComputeShaderRenderPass : ScriptableRenderPass
    {
        private ComputeShaderPostProcess postProcess = null;

        public ComputeShaderRenderPass(RenderPassSettings settings, ComputeShaderPostProcess postProcess)
        {
            Setup(settings, postProcess);
        }

        public void Setup(RenderPassSettings settings, ComputeShaderPostProcess postProcess)
        {
            renderPassEvent = settings.RenderPassEvent;

            this.postProcess = postProcess;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var colorRenderTarget = renderingData.cameraData.renderer.cameraColorTarget;

            ConfigureTarget(colorRenderTarget);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) { }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera != postProcess.Camera)
            {
                return;
            }
            var colorRenderTarget = renderingData.cameraData.renderer.cameraColorTarget;

            postProcess.OnExecuteRenderPass(this, context, colorRenderTarget);
        }
    }
}
#endif