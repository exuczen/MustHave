//#define DEBUG_OFFSETS

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MustHave.Utils;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class UIScreen : UIBehaviour
    {
        public const float IOS_STATUS_BAR_HEIGHT_IN_INCHES = 0.1575f; // = 0.4[cm]/2.54[cm/inch]

        [SerializeField, Range(0f, 1f)]
        private float canvasMatchAspectRatio = 1f;
        [SerializeField]
        private bool setHeaderBackground = default;
        [SerializeField, ConditionalHide("setHeaderBackground", true)]
        private Image headerBackground = default;

        private UICanvas canvas = default;
        private CanvasScaler canvasScaler = default;
        private Canvas parentCanvas = default;
        private RectTransform rectTransform = default;

        public UICanvas Canvas => canvas != null ? canvas : (canvas = transform.GetComponentInParents<UICanvas>());
        public CanvasScaler CanvasScaler => canvasScaler != null ? canvasScaler : (canvasScaler = transform.GetComponentInParents<CanvasScaler>());
        public Canvas ParentCanvas => parentCanvas != null ? parentCanvas : (parentCanvas = transform.GetComponentInParents<Canvas>());
        public RectTransform RectTransform => rectTransform != null ? rectTransform : (rectTransform = transform as RectTransform);
        public bool SetHeaderBackground => setHeaderBackground;

        protected override void Awake()
        {
            OnAwake();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying && CanvasScaler)
            {
                CanvasScaler.matchWidthOrHeight = canvasMatchAspectRatio;
            }
        }

        protected override void OnDisable()
        {
        }

        public void ClearCanvasData()
        {
            canvas = null;
            canvasScaler = null;
        }

        protected virtual void OnAwake() { }

        protected virtual void OnShow() { }

        protected virtual void OnShowInParentCanvas(Canvas parentCanvas, UICanvas activeSceneCanvas) { }

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

        public void ShowInParentCanvas(Canvas parentCanvas, UICanvas activeSceneCanvas)
        {
            this.parentCanvas = parentCanvas;
            this.parentCanvas.gameObject.SetActive(true);
            canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
            canvasScaler.matchWidthOrHeight = canvasMatchAspectRatio;
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
                CanvasScaler.matchWidthOrHeight = canvasMatchAspectRatio;
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

        public T GetCanvasScript<T>() where T : UICanvas
        {
            return Canvas is T ? Canvas as T : null;
        }

        public void SetOffsetsInCanvas(Canvas canvas)
        {
            if (headerBackground)
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
                    headerBackground.rectTransform.offsetMax = new Vector2(0f, topOffest);
                }
            }
        }
    }
}
