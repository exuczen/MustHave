using UnityEngine;
using UnityEngine.EventSystems;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class ContentAnchorFitter : UIBehaviour
    {
        [SerializeField]
        private RectTransform content = default;
        [SerializeField, Tooltip("height / width")]
        private float aspectRatioMin = 1f;
        [SerializeField, Tooltip("height / width")]
        private float aspectRatioMax = 2f;
        [SerializeField, Tooltip("MIN anchor for MIN aspect ratio height / width")]
        private Vector2 aspectRatioMinAnchorMin = Vector2.zero;
        [SerializeField, Tooltip("MAX anchor for MIN aspect ratio height / width")]
        private Vector2 aspectRatioMinAnchorMax = Vector2.one;
        [SerializeField, Tooltip("MIN anchor for MAX aspect ratio height / width")]
        private Vector2 aspectRatioMaxAnchorMin = Vector2.zero;
        [SerializeField, Tooltip("MAX anchor for MAX aspect ratio height / width")]
        private Vector2 aspectRatioMaxAnchorMax = Vector2.one;

        protected override void OnEnable()
        {
            OnRectTransformDimensionsChange();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            //OnRectTransformDimensionsChange();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            if (!EditorApplicationUtils.IsCompilingOrUpdating && enabled && content && Screen.width > 0)
            {
                float aspectRatio = 1f * Screen.height / Screen.width;
                float anchorTransition = Mathf.InverseLerp(aspectRatioMin, aspectRatioMax, aspectRatio);
                Vector2 minAnchor = Vector2.Lerp(aspectRatioMinAnchorMin, aspectRatioMaxAnchorMin, anchorTransition);
                Vector2 maxAnchor = Vector2.Lerp(aspectRatioMinAnchorMax, aspectRatioMaxAnchorMax, anchorTransition);
                content.anchorMin = minAnchor;
                content.anchorMax = maxAnchor;
                content.anchoredPosition = Vector2.zero;
            }
        }
    }
}
