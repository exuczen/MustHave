//#define DEBUG_OFFSETS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utilities;
using MustHave.UI;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class ScreenScript : UIBehaviour
    {
        const float IOS_STATUS_BAR_HEIGHT_IN_INCHES = 0.1575f; // = 0.4[cm]/2.54[cm/inch]

        [SerializeField, Range(0f, 1f)]
        private float _canvasMatchAspectRatio = 1f;
        [SerializeField]
        private bool _setHeaderBackground = default;
        [SerializeField, ConditionalHide("_setHeaderBackground", true)]
        private Image _headerBackground = default;

        private CanvasScript _canvas = default;
        private CanvasScaler _canvasScaler = default;
        private Canvas _parentCanvas = default;
        private RectTransform _rectTransform = default;

        public CanvasScript Canvas => _canvas ?? (_canvas = transform.GetComponentInParents<CanvasScript>());
        public CanvasScaler CanvasScaler { get => _canvasScaler ?? (_canvasScaler = transform.GetComponentInParents<CanvasScaler>()); }
        public Canvas ParentCanvas => _parentCanvas ?? (_parentCanvas = transform.GetComponentInParents<Canvas>());
        public RectTransform RectTransform { get => _rectTransform ?? (_rectTransform = transform as RectTransform); }
        public bool SetHeaderBackground { get => _setHeaderBackground; }

        protected override void Awake()
        {
            OnAwake();
        }

        protected override void OnEnable()
        {
            if (EditorApplicationUtils.IsInEditMode && CanvasScaler)
            {
                CanvasScaler.matchWidthOrHeight = _canvasMatchAspectRatio;
            }
        }

        protected override void OnDisable()
        {
        }

        public void ClearCanvasData()
        {
            _canvas = null;
            _canvasScaler = null;
        }

        protected virtual void OnAwake() { }

        protected virtual void OnShow() { }

        protected virtual void OnShowInParentCanvas(Canvas parentCanvas, CanvasScript activeSceneCanvas) { }

        protected virtual void OnHide() { }

        public virtual void OnCanvasRectTransformDimensionsChange(Canvas canvas) { }

        public virtual bool OnBack() { return true; }

        public void OnBackButtonClick()
        {
            if (OnBack())
            {
                Canvas.BackToPrevScreen();
            }
        }

        public void ShowInParentCanvas(Canvas parentCanvas, CanvasScript activeSceneCanvas)
        {
            _parentCanvas = parentCanvas;
            _parentCanvas.gameObject.SetActive(true);
            _canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
            _canvasScaler.matchWidthOrHeight = _canvasMatchAspectRatio;
            SetOffsetsInCanvas(parentCanvas);
            transform.SetParent(parentCanvas.transform, false);
            gameObject.SetActive(true);
            OnShowInParentCanvas(parentCanvas, activeSceneCanvas);
        }

        public void Show()
        {
            if (Canvas)
            {
                SetOffsetsInCanvas(Canvas.Canvas);
                Canvas.ActiveScreen = this;
                CanvasScaler.matchWidthOrHeight = _canvasMatchAspectRatio;
            }
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            if (Canvas && this == Canvas.ActiveScreen)
                Canvas.ActiveScreen = null;
            OnHide();
            gameObject.SetActive(false);
        }

        public T GetCanvasScript<T>() where T : CanvasScript
        {
            return Canvas is T ? Canvas as T : null;
        }

        public void SetOffsetsInCanvas(Canvas canvas)
        {
            if (_headerBackground)
            {
#if UNITY_IOS
            float topOffsetInInches = IOS_STATUS_BAR_HEIGHT_IN_INCHES;
#elif DEBUG_OFFSETS
            float topOffsetInInches = IOS_STATUS_BAR_HEIGHT_IN_INCHES;
#else
                float topOffsetInInches = 0f;
#endif
                if (topOffsetInInches > 0f)
                {
                    float topOffest;
                    if (Screen.dpi >= 1f)
                    {
                        topOffest = topOffsetInInches * Screen.dpi;
                    }
                    else
                    {
                        topOffest = 0.02f * Screen.height;
                    }
                    topOffest /= canvas.scaleFactor;
                    //Debug.Log(GetType() + ".SetOffsets: " + Screen.height + " " + Screen.currentResolution.height + " " + " " + CanvasScaler.scaleFactor.ToString("n2") + " " + canvas.scaleFactor.ToString("n2") + " " + Screen.dpi);
                    RectTransform.offsetMax = new Vector2(0f, -topOffest);
                    _headerBackground.rectTransform.offsetMax = new Vector2(0f, topOffest);
                }
            }
        }
    }
}
