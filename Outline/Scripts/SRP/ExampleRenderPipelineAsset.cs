using UnityEngine;
using UnityEngine.Rendering;

namespace MustHave
{
    [CreateAssetMenu(menuName = "MustHave/Rendering/ExampleRenderPipelineAsset")]
    public class ExampleRenderPipelineAsset : RenderPipelineAsset
    {
        // Unity calls this method before rendering the first frame.
        // If a setting on the Render Pipeline Asset changes, Unity destroys the current Render Pipeline Instance and calls this method again before rendering the next frame.
        protected override RenderPipeline CreatePipeline()
        {
            // Instantiate the Render Pipeline that this custom SRP uses for rendering.
            return new ExampleRenderPipeline();
        }
    }
}
