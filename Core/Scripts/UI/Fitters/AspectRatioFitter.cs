﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.AspectRatioFitter;

namespace MustHave.UI
{
    /// <summary>
    /// This class duplicates functionality of UnityEngine.UI.AspectRatioFitter tu suppress unity inspector warning on original fitter added to the child of layout group:
    /// "Parent has a type of layout group component. A child of a layout group should not have a Aspect Ratio Fitter component, since it should be driven by the layout group."
    /// </summary>
    [ExecuteInEditMode]
    public class AspectRatioFitter : UIBehaviour
    {
        [SerializeField]
        private AspectMode aspectMode = default;
        [SerializeField, Tooltip("width / height")]
        private float aspectRatio = 1f;

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
            if (!EditorApplicationUtils.IsCompilingOrUpdating && enabled && aspectRatio > float.MinValue)
            {
                RectTransform rectTransform = transform as RectTransform;
                switch (aspectMode)
                {
                    case AspectMode.None:
                        break;
                    case AspectMode.WidthControlsHeight:
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x / aspectRatio);
                        break;
                    case AspectMode.HeightControlsWidth:
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y * aspectRatio, rectTransform.sizeDelta.y);
                        break;
                    case AspectMode.FitInParent:
                        throw new NotImplementedException();
                    case AspectMode.EnvelopeParent:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}
