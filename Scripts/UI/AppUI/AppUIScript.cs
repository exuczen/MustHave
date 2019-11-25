using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MustHave.Utilities;
using MustHave.DesignPatterns;

namespace MustHave.UI
{
    [RequireComponent(typeof(Canvas))]
    public class AppUIScript : PersistentCanvas<AppUIScript>
    {
        [SerializeField]
        private List<MessageBus> _sceneMessageBusList = default;
        [SerializeField]
        private MessageBus _appMessageBus = default;
        [SerializeField]
        private ShowScreenMessageEvent _showScreenMessage = default;
        [SerializeField]
        private MessageEvent _backMessage = default;
        [SerializeField]
        private SetAlertPopupMessageEvent _setAlertPopupMessage = default;
        [SerializeField]
        private Image _screenshotImage = default;
        [SerializeField]
        private AlertPopupScript _alertPopup = default;
        [SerializeField]
        private ProgressSpinnerPanel _progressSpinnerPanel = default;

        private List<ScreenData> _screenDataStack = new List<ScreenData>();
        private string _activeSceneName = default;
        private CanvasScript _activeCanvas = default;
        private ScreenData _activeScreenData = default;
        private ScreenData _loadingSceneScreenData = default;
        private ISceneChangeListener _sceneChangeListener = default;
        private float _sceneLoadingStartTime = -1f;
        private Dictionary<Type, ScreenScript> _screensDict = new Dictionary<Type, ScreenScript>();
        private AlertPopupScript _activeAlertPopup = default;

        public AlertPopupScript ActiveAlertPopup { get => _activeAlertPopup; }
        public ProgressSpinnerPanel ProgressSpinnerPanel { get => _progressSpinnerPanel; }

        protected override void OnAwake()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.sceneLoaded += OnSceneLoaded;
            //SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            _appMessageBus.Register(OnNotify);
            _activeSceneName = activeScene.name;

            AlertPopupScript[] alertPopups = GetComponentsInChildren<AlertPopupScript>(true);
            foreach (var popup in alertPopups)
            {
                popup.Init(this);
                popup.Hide();
            }
            _activeAlertPopup = _alertPopup;

            foreach (Transform child in transform)
            {
                ScreenScript screen = child.GetComponent<ScreenScript>();
                if (screen)
                {
                    _screensDict.Add(screen.GetType(), screen);
                    screen.gameObject.SetActive(false);
                }
            }
            List<CanvasScript> canvasList = SceneUtils.FindObjectsOfType<CanvasScript>(activeScene, true);
            _activeCanvas = canvasList.Find(canvas => canvas.ActiveOnAppAwake);
            if (_activeCanvas)
                SetPersistentComponentsParent(_activeCanvas.TopLayer);

            foreach (var canvas in canvasList)
            {
                canvas.Init();
            }
            foreach (var canvas in canvasList)
            {
                canvas.OnAppAwake();
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0; // Other values will cause to ignore targetFrameRate
        }

        private void OnNotify(MessageEvent obj)
        {
            if (_showScreenMessage.Equals(obj))
            {
                ShowScreen(_showScreenMessage.Data);
            }
            else if (_backMessage.Equals(obj))
            {
                BackToPrevScreen();
            }
            else if (_setAlertPopupMessage.Equals(obj))
            {
                SetActiveAlertPopup(_setAlertPopupMessage.Data);
            }
        }

        private void OnScenePreload(string sceneName)
        {
            _sceneMessageBusList.ForEach(bus => bus.Clear());

            // Clear screen objects in stack
            foreach (var screenData in _screenDataStack)
            {
                if (screenData.Screen)
                    screenData.Screen.ClearCanvasData();
                screenData.Screen = null;
            }
            _sceneChangeListener?.OnScenePreload(sceneName);
            _sceneLoadingStartTime = Time.time;
        }

        private void OnSceneLoadingProgress(float progress)
        {
        }

        private IEnumerator OnSceneLoadedRoutine(Scene scene)
        {
            if (_loadingSceneScreenData != null)
            {
                ScreenData loadedScreenData = _loadingSceneScreenData;
                SceneManager.SetActiveScene(scene);
                _activeSceneName = scene.name;
                _loadingSceneScreenData = null;
                List<CanvasScript> canvasList = SceneUtils.FindObjectsOfType<CanvasScript>(scene, true);
                foreach (var canvas in canvasList)
                {
                    canvas.Init();
                    canvas.gameObject.SetActive(false);
                }
                void onEnd()
                {
                    ShowScreen(loadedScreenData);
                    _screenshotImage.gameObject.SetActive(false);
                    _progressSpinnerPanel.Hide();
                    _sceneLoadingStartTime = -1f;
                }
                if (_sceneChangeListener != null)
                {
                    float sceneLoadingDuration = _sceneLoadingStartTime > 0f ? Time.time - _sceneLoadingStartTime : 0f;
                    yield return _sceneChangeListener?.OnSceneLoadedRoutine(scene, sceneLoadingDuration, onEnd);
                    _sceneChangeListener = null;
                }
                else
                {
                    onEnd();
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log(GetType() + ".OnSceneLoaded: " + scene.name);
            StartCoroutine(OnSceneLoadedRoutine(scene));
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            //Debug.Log(GetType() + ".OnActiveSceneChanged: from " + oldScene.name + " to " + newScene.name);
            _activeSceneName = newScene.name;
        }

        private IEnumerator OnSceneCloseRoutine(Action onSuccess)
        {
            SetPersistentComponentsParent(transform);
            if (_activeCanvas && _activeScreenData != null && _activeScreenData.Screen)
            {
                _activeAlertPopup = _alertPopup;
                bool showProgressSpinner = true;
                _sceneChangeListener?.OnSceneClose(_activeSceneName, out showProgressSpinner);
                _activeCanvas.StopAllCoroutines();
                _activeScreenData.Screen.gameObject.SetActive(true);
                // Take and show screenshot
                yield return new WaitForEndOfFrame();
                Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
                _screenshotImage.sprite = TextureUtils.CreateSpriteFromTexture(texture);
                _screenshotImage.gameObject.SetActive(true);
                if (showProgressSpinner)
                    _progressSpinnerPanel.Show();
                _activeScreenData.Screen.Hide();
                _activeCanvas.Hide();
                // Clear data
                _activeSceneName = null;
                _activeCanvas = null;
                _activeScreenData = null;
                // Invoke callback
                if (onSuccess != null)
                    onSuccess.Invoke();
            }
        }

        private void OnSceneClose(Action onSuccess)
        {
            StartCoroutine(OnSceneCloseRoutine(onSuccess));
        }

        private void ShowScreen(ScreenData screenData)
        {
            if (/*SceneUtils.IsLoadingScene || */string.IsNullOrEmpty(_activeSceneName))
            {
                return;
            }
            if (_activeScreenData != null)
            {
                if (_activeScreenData.Screen == screenData.Screen)
                {
                    return;
                }
                else if (_activeScreenData.KeepOnStack)
                {
                    _screenDataStack.Add(_activeScreenData);
                }
                if (_activeScreenData.Screen && _activeSceneName.Equals(screenData.SceneName))
                {
                    _activeScreenData.Screen.Hide();
                }
            }
            if (screenData != null)
            {
                if (screenData.ClearStack)
                {
                    _screenDataStack.Clear();
                }
                else
                {
                    int screenIndexInStack = _screenDataStack.FindIndex(data => data.ScreenType == screenData.ScreenType);
                    //_screenDataStack.Select(data => data.ScreenType).ToList().Print(".ShowScreen: AppUI.stack: ");
                    if (screenIndexInStack >= 0)
                    {
                        _screenDataStack.RemoveRange(screenIndexInStack, _screenDataStack.Count - screenIndexInStack);
                    }
                }

                if (screenData.CanvasType == this.GetType())
                {
                    ScreenScript screen = screenData.Screen ?? GetScreen(screenData.ScreenType);
                    if (screen)
                    {
                        _sceneChangeListener = screen.GetComponent<ISceneChangeListener>();
                        _activeCanvas.Hide();
                        screen.ShowInParentCanvas(GetComponent<Canvas>(), _activeCanvas);
                    }
                }
                else
                {
                    ScreenScript screen = screenData.Screen;
                    //Debug.Log(GetType() + ".ShowScreen: " + screenData.CanvasType + "." + screenData.ScreenType + " " + screen);

                    _progressSpinnerPanel.transform.SetParent(transform, false);
                    _activeAlertPopup.transform.SetParent(transform, false);

                    if (!_activeSceneName.Equals(screenData.SceneName))
                    {
                        //Debug.Log(GetType() + ".ShowScreen: load new scene " + screenData.SceneName + " from " + _activeScreenData.ScreenType);
                        OnSceneClose(() => {
                            _loadingSceneScreenData = screenData;
                            SceneUtils.LoadSceneAsync(this, screenData.SceneName, LoadSceneMode.Single, OnScenePreload);
                        });
                        return;
                    }
                    else if (_activeCanvas && _activeCanvas.GetType() != screenData.CanvasType)
                    {
                        // Hide old canvas
                        _activeCanvas.StopAllCoroutines();
                        _activeCanvas.SetProgressSpinnerPanel(null);
                        _activeCanvas.SetAlertPopup(null);
                        _activeCanvas.Hide();
                    }
                    if (screen && screen.Canvas)
                    {
                        _activeCanvas = screen.Canvas;
                    }
                    else
                    {
                        _activeCanvas = FindCanvasInActiveScene(screenData.CanvasType);
                        SetPersistentComponentsParent(_activeCanvas.TopLayer);
                        screen = screen ?? _activeCanvas.GetScreen(screenData.ScreenType);
                        //Debug.Log(GetType() + ".ShowScreen: found screen: " + screen + " " + screen.Canvas);
                        screenData = new ScreenData(screen, screenData.KeepOnStack, screenData.ClearStack);
                    }
                    _activeScreenData = screenData;
                    _activeCanvas.SetProgressSpinnerPanel(_progressSpinnerPanel);
                    _activeCanvas.SetAlertPopup(_activeAlertPopup);
                    _activeCanvas.Show();
                    screen.Show();
                }
            }
        }

        public ScreenScript GetScreen(Type screenType)
        {
            if (_screensDict.TryGetValue(screenType, out ScreenScript screen))
            {
                return screen;
            }
            return null;
        }

        private CanvasScript FindCanvasInActiveScene(Type canvasType)
        {
            List<CanvasScript> canvasList = SceneUtils.FindObjectsOfType<CanvasScript>(SceneManager.GetActiveScene(), true);
            return canvasList.Find(c => c.GetType() == canvasType);
        }

        private void SetAppCanvasRenderMode(RenderMode renderMode)
        {
            Canvas appCanvas = GetComponent<Canvas>();
            appCanvas?.SetRenderMode(renderMode);
        }

        private void SetActiveAlertPopup(Type type)
        {
            _activeAlertPopup = (_activeCanvas?.TopLayer.GetComponentInChildren(type, true) ?? transform.GetComponentInChildren(type, true)) as AlertPopupScript;
            //Debug.Log(GetType() + ".SetActiveAlertPopup: _activeAlertPopup=" + _activeAlertPopup + " _activeCanvas=" + _activeCanvas);
            if (_activeAlertPopup && _activeCanvas)
            {
                _activeCanvas.SetAlertPopup(_activeAlertPopup);
            }
        }

        private void BackToPrevScreen()
        {
            if (_screenDataStack.Count > 0)
            {
                if (_activeScreenData != null)
                {
                    _activeScreenData.KeepOnStack = false;
                }
                ShowScreen(_screenDataStack.PickLastElement());
            }
            else
            {
#if UNITY_ANDROID || UNITY_EDITOR
                _activeAlertPopup.ShowQuitWarning();
#endif
            }
        }

        public void HACKAddScreenDataToScreenStack<T1, T2>(string sceneName) where T1 : ScreenScript where T2 : CanvasScript
        {
            _screenDataStack.Add(new ScreenData(typeof(T1), typeof(T2), sceneName));
        }

#if UNITY_ANDROID || UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_activeAlertPopup.IsShown)
                {
                    _activeAlertPopup.OnDismissButtonClick();
                }
                else if (_activeScreenData != null && _activeScreenData.Screen && _activeScreenData.Screen.OnBack())
                {
                    BackToPrevScreen();
                }
            }
        }
#endif
    }
}
