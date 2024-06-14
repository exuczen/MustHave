using UnityEngine;

namespace MustHave
{
    public class RendererData
    {
        public Renderer Renderer => renderer;

        private Renderer renderer;
        private Material sharedMaterial;
        private uint layerMask;
        private int layer;

        public void Clear()
        {
            renderer = null;
            sharedMaterial = null;
            layerMask = 0;
            layer = 0;
        }

        public void SetRenderer(Renderer renderer)
        {
            this.renderer = renderer;
            sharedMaterial = renderer.sharedMaterial;
            layerMask = renderer.renderingLayerMask;
            layer = renderer.gameObject.layer;
        }

        public void Setup(Material material, int layer)
        {
            renderer.gameObject.layer = layer;
            renderer.sharedMaterial = material;
        }

        public void Restore()
        {
            renderer.gameObject.layer = layer;
            renderer.sharedMaterial = sharedMaterial;
        }
    }
}
