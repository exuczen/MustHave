using UnityEngine;
using UnityEngine.UI;
using MustHave.Utilities;

namespace MustHave.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ListItem : MonoBehaviour
    {
        [SerializeField] protected Image image = null;
        [SerializeField] protected Image background = null;
        [SerializeField] protected Animator progressSpinner = null;

        public RectTransform RectTransform => transform as RectTransform;

        public virtual void SetImageTexture(Texture2D texture, bool backgroundEnabled)
        {
            if (texture)
            {
                image.sprite = TextureUtils.CreateSpriteFromTexture(texture);
                image.gameObject.SetActive(true);
                background.enabled = backgroundEnabled;
                progressSpinner.gameObject.SetActive(false);
            }
        }
    } 
}
