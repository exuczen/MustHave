using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MustHave.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UICanvas : UIBehaviour
    {
        [SerializeField] private bool activeOnAppAwake = false;
        [SerializeField] private AppMessageEvents appMessages = default;

        [SerializeField] private RectTransform topLayer = default;

        private readonly Dictionary<Type, UIScreen> screensDict = new Dictionary<Type, UIScreen>();
        private string sceneName = default;
        private AlertPopup alertPopup = default;
        private ProgressSpinnerPanel progressSpinnerPanel = default;
        private Canvas canvas = default;

        public string SceneName => string.IsNullOrEmpty(sceneName) ? (sceneName = SceneUtils.ActiveSceneName) : sceneName;
        public AlertPopup AlertPopup => alertPopup;
        public ProgressSpinnerPanel ProgressSpinnerPanel => progressSpinnerPanel;
        public Canvas Canvas => canvas != null ? canvas : (canvas = GetComponent<Canvas>());
        public UIScreen ActiveScreen { get; set; } = default;
        public RectTransform TopLayer => topLayer;
        public bool ActiveOnAppAwake => activeOnAppAwake;

        protected override void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake() { }

        protected virtual void OnInit() { }

        protected virtual void OnAppAwake(bool active) { }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }

        public void OnAppAwake()
        {
            if (activeOnAppAwake)
            {
                Show();
            }
            OnAppAwake(activeOnAppAwake);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            //List<ScreenScript> screens = _screensDict.Values.ToList();
            //ScreenScript activeScreen = screens.Find(screen => screen.gameObject.activeSelf);
            if (ActiveScreen)
            {
                ActiveScreen.SetOffsetsInCanvas(Canvas);
                ActiveScreen.OnCanvasRectTransformDimensionsChange(Canvas);
            }
        }

        public void Init()
        {
            canvas = Canvas;
            sceneName = SceneName;
            screensDict.Clear();
            //ScreenScript[] screens = GetComponentsInChildren<ScreenScript>();
            //foreach (var screen in screens)
            //{
            //    _screensDict.Add(screen.GetType(), screen);
            //}
            foreach (Transform child in transform)
            {
                UIScreen screen = child.GetComponent<UIScreen>();
                if (screen)
                {
                    screensDict.Add(screen.GetType(), screen);
                    screen.gameObject.SetActive(false);
                }
            }
            OnInit();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnHide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1">screen type</typeparam>
        /// <typeparam name="T2">canvas type</typeparam>
        /// <param name="keepOnStack"></param>
        public void ShowScreen<T1, T2>(bool keepOnStack = true, bool clearStack = false) where T1 : UIScreen where T2 : UICanvas
        {
            if (typeof(T2) == GetType())
            {
                ShowScreen<T1>(keepOnStack, clearStack);
            }
            else
            {
                ShowScreen(new ScreenData(typeof(T1), typeof(T2), SceneUtils.ActiveSceneName, keepOnStack, clearStack));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1">screen type</typeparam>
        /// <typeparam name="T2">canvas type</typeparam>
        /// <param name="sceneName"></param>
        /// <param name="keepOnStack"></param>
        public void ShowScreenFromOtherScene<T1, T2>(Enum sceneName, bool keepOnStack = true, bool clearStack = false) where T1 : UIScreen where T2 : UICanvas
        {
            ShowScreen(new ScreenData(typeof(T1), typeof(T2), sceneName.ToString(), keepOnStack, clearStack));
        }

        public void ShowScreenFromAppUI<T>() where T : UIScreen
        {
            ShowScreen(new ScreenData(typeof(T), typeof(AppUI), SceneUtils.ActiveSceneName, false, false));
        }

        public void ShowScreen<T>(bool keepOnStack = true, bool clearStack = false)
        {
            //Debug.Log(GetType() + ".ShowScreen: " + typeof(T));
            //foreach (var kvp in _screensDict)
            //{
            //    Debug.Log(GetType() + ".ShowScreen: " + kvp.Key);
            //}
            Type screenType = typeof(T);
            if (screensDict.TryGetValue(screenType, out UIScreen screen) && screen)
            {
                ShowScreen(screen, keepOnStack, clearStack);
            }
        }

        public void ShowScreen(UIScreen screen, bool keepOnStack = true, bool clearStack = false)
        {
            ShowScreen(new ScreenData(screen, keepOnStack, clearStack));
        }

        public void ShowScreen(ScreenData screenData)
        {
            appMessages.ShowScreenMessage.Invoke(screenData);
        }

        public UIScreen GetScreen(Type screenType)
        {
            //Debug.Log(GetType() + ".GetScreen: " + screenType);
            //foreach (var item in _screensDict)
            //{
            //    Debug.Log(GetType() + ".GetScreen: _screensDict:" + item.Value);
            //}
            if (screensDict.TryGetValue(screenType, out UIScreen screen))
            {
                return screen;
            }
            return null;
        }

        public T GetScreen<T>() where T : UIScreen
        {
            UIScreen screen = GetScreen(typeof(T));
            return screen ? screen as T : null;
        }

        public void BackToPrevScreen()
        {
            appMessages.BackToPrevScreenMessage.Invoke();
        }

        public void SetProgressSpinnerPanel(ProgressSpinnerPanel progressSpinnerPanel)
        {
            this.progressSpinnerPanel = progressSpinnerPanel;
            if (this.progressSpinnerPanel)
                this.progressSpinnerPanel.transform.SetParent(topLayer, false);
        }

        public void SetAlertPopup(AlertPopup alertPopup)
        {
            this.alertPopup = alertPopup;
            if (this.alertPopup)
                this.alertPopup.transform.SetParent(topLayer, false);
        }

        public void SetAlertPopup<T>()
        {
            appMessages.SetAlertPopupMessage.Invoke(typeof(T));
        }
    }
}
