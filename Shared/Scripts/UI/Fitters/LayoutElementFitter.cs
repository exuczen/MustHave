using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utils;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class LayoutElementFitter : UIBehaviour
    {
        [SerializeField]
        private LayoutElement layoutElement = default;
        [SerializeField, Tooltip("height / width")]
        private float aspectRatioMin = default;
        [SerializeField, Tooltip("height / width")]
        private float aspectRatioMax = default;
        [SerializeField]
        private float prefferedWidthMin = default;
        [SerializeField]
        private float prefferedWidthMax = default;

        protected override void OnEnable()
        {
            OnRectTransformDimensionsChange();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            OnRectTransformDimensionsChange();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            if (!EditorApplicationUtils.IsCompilingOrUpdating && enabled && layoutElement)
            {
                float aspectRatio = 1f * Screen.height / Screen.width;
                float aspectTransition = Mathf.InverseLerp(aspectRatioMin, aspectRatioMax, aspectRatio);
                //Debug.Log(GetType() + "." + aspectTransition + " " + Maths.LerpInverse(_aspectRatioMin, _aspectRatioMax, aspectRatio));
                float prefferedWidth = Mathf.Lerp(prefferedWidthMin, prefferedWidthMax, aspectTransition);
                layoutElement.preferredWidth = prefferedWidth;
            }
        }
    }


}
