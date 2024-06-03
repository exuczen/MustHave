#if UNITY_PIPELINE_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MustHave
{
    public class ComputeShaderRenderPass : ScriptableRenderPass
    {
        //private RenderTargetIdentifier colorRenderTarget;
        //private RenderTargetIdentifier sourceRenderTarget;
        //private RenderTargetIdentifier outputRenderTarget;

        private ComputeShaderPostProcess postProcess = null;
        //private RenderTargetHandle renderTargetHandle;

        //private RenderTextureDescriptor rtDescriptor;
        //private RTHandle rtHandle;

        public ComputeShaderRenderPass(RenderPassSettings settings, ComputeShaderPostProcess postProcess)
        {
            Init(settings, postProcess);
        }

        public void Init(RenderPassSettings settings, ComputeShaderPostProcess postProcess)
        {
            renderPassEvent = settings.RenderPassEvent;

            //sourceRenderTarget = new RenderTargetIdentifier(postProcess.SourceTexture);
            //outputRenderTarget = new RenderTargetIdentifier(postProcess.OutputTexture);

            this.postProcess = postProcess;

            //renderTargetHandle.Init("_OutlineRenderPassHandle");
            //rtDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var colorRenderTarget = renderingData.cameraData.renderer.cameraColorTarget;

            ConfigureTarget(colorRenderTarget);

            //var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            //cmd.GetTemporaryRT(renderTargetHandle.id, descriptor, FilterMode.Point);

            //Debug.Log($"{GetType().Name}.OnCameraSetup");
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            //cmd?.ReleaseTemporaryRT(renderTargetHandle.id);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            //rtDescriptor.width = cameraTextureDescriptor.width;
            //rtDescriptor.height = cameraTextureDescriptor.height;

            //RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textureDescriptor);

            //Debug.Log($"{GetType().Name}.Configure: {cameraTextureDescriptor.width}");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera != postProcess.Camera)
            {
                return;
            }
            CommandBuffer cmd = CommandBufferPool.Get();

            //Debug.Log($"{GetType().Name}.{renderingData.cameraData.renderer.cameraColorTarget == this.colorRenderTarget}");
            //Debug.Log($"{GetType().Name}.{renderingData.cameraData.camera}");

            var colorRenderTarget = renderingData.cameraData.renderer.cameraColorTarget;

            //Blit(cmd, colorRenderTarget, renderTargetHandle.Identifier());
            //Blit(cmd, renderTargetHandle.Identifier(), colorRenderTarget);

            //sourceRenderTarget = new RenderTargetIdentifier(postProcess.SourceTexture);
            //outputRenderTarget = new RenderTargetIdentifier(postProcess.OutputTexture);

            //postProcess.SetupOnRenderImage(cmd);

            //Blit(cmd, colorRenderTarget, sourceRenderTarget);

            //postProcess.DispatchShader(cmd);

            //Blit(cmd, outputRenderTarget, colorRenderTarget);

            //context.ExecuteCommandBuffer(cmd);

            if (postProcess.OnExecuteRenderPass(this, cmd, colorRenderTarget))
            {
                context.ExecuteCommandBuffer(cmd);
            }
            cmd.Clear();
            CommandBufferPool.Release(cmd);

            //if (!Application.isPlaying)
            //{
            //    Debug.Log($"{GetType().Name}.Execute: Application is not Playing");
            //}
        }

        //public void Dispose()
        //{
        //    Debug.Log($"{GetType().Name}.Dispose");
        //}
    }
}
#endif