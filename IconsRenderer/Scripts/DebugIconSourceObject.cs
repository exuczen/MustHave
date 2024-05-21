using UnityEngine;

namespace MustHave.IconsRenderer
{
    public class DebugIconSourceObject : IconSourceObject
    {
        [SerializeField]
        private Color debugColor = Color.white;
        [SerializeField]
        private MeshRenderer meshRenderer = null;

        public override void OnPreRender()
        {
            var material = Application.isPlaying ? meshRenderer.material : meshRenderer.sharedMaterial;
            material.color = debugColor;
        }
    }
}
