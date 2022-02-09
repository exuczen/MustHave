using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using MustHave.Utils;

namespace MustHave.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasScript : UIBehaviour
    {
        [SerializeField] private bool _activeOnAppAwake = false;
        [SerializeField] private AppMessageEvents _appMessages = default;

        [SerializeField] private RectTransform _topLayer = default;

        private Dictionary<Type, ScreenScript> _screensDict = new Dictionary<Type, ScreenScript>();
        private string _sceneName = default;
        private AlertPopup _alertPopup = default;
        private ProgressSpinnerPanel _progressSpinnerPanel = default;
        private Canvas _canvas = default;
        private ScreenScript _activeScreen = default;

        public string SceneName { get => string.IsNullOrEmpty(_sceneName) ? (_sceneName = SceneUtils.ActiveSceneName) : _sceneName; }
        public AlertPopup AlertPopup { get => _alertPopup; }
        public ProgressSpinnerPanel ProgressSpinnerPanel { get => _progressSpinnerPanel; }
        public Canvas Canvas { get => _canvas ?? (_canvas = GetComponent<Canvas>()); }
        public ScreenScript ActiveScreen { get => _activeScreen; set => _activeScreen = value; }
        public RectTransform TopLayer { get => _topLayer; }
        public bool ActiveOnAppAwake { get => _activeOnAppAwake; }

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
            if (_activeOnAppAwake)
            {
                Show();
            }
            OnAppAwake(_activeOnAppAwake);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            //List<ScreenScript> screens = _screensDict.Values.ToList();
            //ScreenScript activeScreen = screens.Find(screen => screen.gameObject.activeSelf);
            if (_activeScreen)
            {
                _activeScreen.SetOffsetsInCanvas(Canvas);
                _activeScreen.OnCanvasRectTransformDimensionsChange(Canvas);
            }
        }

        public void Init()
        {
            _canvas = Canvas;
            _sceneName = SceneName;
            _screensDict.Clear();
            //ScreenScript[] screens = GetComponentsInChildren<ScreenScript>();
            //foreach (var screen in screens)
            //{
            //    _screensDict.Add(screen.GetType(), screen);
            //}
            foreach (Transform child in transform)
            {
                ScreenScript screen = child.GetComponent<ScreenScript>();
                if (screen)
                {
                    _screensDict.Add(screen.GetType(), screen);
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
        public void ShowScreen<T1, T2>(bool keepOnStack = true, bool clearStack = false) where T1 : ScreenScript where T2 : CanvasScript
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
        public void ShowScreenFromOtherScene<T1, T2>(Enum sceneName, bool keepOnStack = true, bool clearStack = false) where T1 : ScreenScript where T2 : CanvasScript
        {
            ShowScreen(new ScreenData(typeof(T1), typeof(T2), sceneName.ToString(), keepOnStack, clearStack));
        }

        public void ShowScreenFromAppUI<T>() where T : ScreenScript
        {
            ShowScreen(new ScreenData(typeof(T), typeof(AppUIScript), SceneUtils.ActiveSceneName, false, false));
        }

        public void ShowScreen<T>(bool keepOnStack = true, bool clearStack = false)
        {
            //Debug.Log(GetType() + ".ShowScreen: " + typeof(T));
            //foreach (var kvp in _screensDict)
            //{
            //    Debug.Log(GetType() + ".ShowScreen: " + kvp.Key);
            //}
            Type screenType = typeof(T);
            if (_screensDict.TryGetValue(screenType, out ScreenScript screen) && screen)
            {
                ShowScreen(screen, keepOnStack, clearStack);
            }
        }

        public void ShowScreen(ScreenScript screen, bool keepOnStack = true, bool clearStack = false)
        {
            ShowScreen(new ScreenData(screen, keepOnStack, clearStack));
        }

        public void ShowScreen(ScreenData screenData)
        {
            _appMessages.ShowScreenMessage.Invoke(screenData);
        }

        public ScreenScript GetScreen(Type screenType)
        {
            //Debug.Log(GetType() + ".GetScreen: " + screenType);
            //foreach (var item in _screensDict)
            //{
            //    Debug.Log(GetType() + ".GetScreen: _screensDict:" + item.Value);
            //}
            if (_screensDict.TryGetValue(screenType, out ScreenScript screen))
            {
                return screen;
            }
            return null;
        }

        public T GetScreen<T>() where T : ScreenScript
        {
            ScreenScript screen = GetScreen(typeof(T));
            return screen ? screen as T : null;
        }

        public void BackToPrevScreen()
        {
            _appMessages.BackToPrevScreenMessage.Invoke();
        }

        public void SetProgressSpinnerPanel(ProgressSpinnerPanel progressSpinnerPanel)
        {
            _progressSpinnerPanel = progressSpinnerPanel;
            if (_progressSpinnerPanel)
                _progressSpinnerPanel.transform.SetParent(_topLayer, false);
        }

        public void SetAlertPopup(AlertPopup alertPopup)
        {
            _alertPopup = alertPopup;
            if (_alertPopup)
                _alertPopup.transform.SetParent(_topLayer, false);
        }

        public void SetAlertPopup<T>()
        {
            _appMessages.SetAlertPopupMessage.Invoke(typeof(T));
        }
    }
}
