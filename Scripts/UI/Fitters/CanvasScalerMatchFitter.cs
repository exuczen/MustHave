using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utils;

namespace MustHave.UI
{
    /// <summary>
    /// Works only for CanvasScaler.ScreenMatchMode.MatchWidthOrHeight
    /// </summary>
    [ExecuteInEditMode]
    public class CanvasScalerMatchFitter : UIBehaviour
    {
        [SerializeField]
        private CanvasScaler canvasScaler = default;
        [SerializeField, Tooltip("height / width")]
        private float aspectRatioMin = default;
        [SerializeField, Tooltip("height / width")]
        private float aspectRatioMax = default;
        [SerializeField, Range(0f, 1f), Tooltip("Match for MIN aspect ratio height / width")]
        private float aspectRatioMinMatch = 0f;
        [SerializeField, Range(0f, 1f), Tooltip("Match for MAX aspect ratio height / width")]
        private float aspectRatioMaxMatch = 1f;

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
            if (!EditorApplicationUtils.IsCompilingOrUpdating && enabled
                && canvasScaler && canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                float aspectRatio = 1f * Screen.height / Screen.width;
                float anchorTransition = Mathf.InverseLerp(aspectRatioMin, aspectRatioMax, aspectRatio);
                float match = Mathf.Lerp(aspectRatioMinMatch, aspectRatioMaxMatch, anchorTransition);
                canvasScaler.matchWidthOrHeight = match;
            }
        }
    }
}
