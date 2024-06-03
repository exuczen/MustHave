#if UNITY_PIPELINE_URP
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MustHave
{
    public class ComputeShaderRendererFeature : ScriptableRendererFeature
    {
        private ComputeShaderRenderPass RenderPass { get; set; }

        [SerializeField]
        private RenderPassSettings settings = new();

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(RenderPass);
        }

        public override void Create()
        {
            RenderPass = new(settings, null);
        }
    }
}
#endif