using System.Collections;
using UnityEngine;

namespace MustHave
{
    public class IconSourceObject : MonoBehaviour
    {
        public Sprite Sprite { get => sprite; set => sprite = value; }
        public BoxCollider BoxCollider { get => boxCollider; set => boxCollider = value; }

        [SerializeField]
        private BoxCollider boxCollider = null;
        [SerializeField]
        private Sprite sprite = null;

        public virtual IconSourceObject CreateInstance(Transform parent)
        {
            return Instantiate(this, parent);
        }

        public virtual void OnPreRender() { }
    }
}