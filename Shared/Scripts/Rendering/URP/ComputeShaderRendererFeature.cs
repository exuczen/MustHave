#if UNITY_PIPELINE_URP
using System;
using UnityEngine.Rendering.Universal;

namespace MustHave
{
    public class ComputeShaderRendererFeature : ScriptableRendererFeature
    {
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            throw new NotImplementedException("ComputeShaderRenderPass is created and added to render queue by ComputeShaderPostProcess");
        }

        public override void Create()
        {
            throw new NotImplementedException("ComputeShaderRenderPass is created and added to render queue by ComputeShaderPostProcess");
        }
    }
}
#endif