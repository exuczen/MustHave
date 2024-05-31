using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MustHave.Utils;

namespace MustHave.UI
{
    [RequireComponent(typeof(Canvas))]
    public class AppUI : PersistentCanvas<AppUI>
    {
        [SerializeField] private List<MessageEventGroup> sceneMessageGroups = default;
        [SerializeField] private AppMessageEvents appMessages = default;
        [SerializeField] private Image screenshotImage = default;
        [SerializeField] private AlertPopup alertPopup = default;
        [SerializeField] private ProgressSpinnerPanel progressSpinnerPanel = default;

        private readonly List<ScreenData> screenDataStack = new();
        private readonly Dictionary<Type, UIScreen> screensDict = new();
        private string activeSceneName = default;
        private UICanvas activeCanvas = default;
        private ScreenData activeScreenData = default;
        private ScreenData loadingSceneScreenData = default;
        private ISceneChangeListener sceneChangeListener = default;
        private float sceneLoadingStartTime = -1f;
        private AlertPopup activeAlertPopup = default;

        public AlertPopup ActiveAlertPopup => activeAlertPopup;
        public ProgressSpinnerPanel ProgressSpinnerPanel => progressSpinnerPanel;

        protected override void OnAwake()
        {
            base.OnAwake();

            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.sceneLoaded += OnSceneLoaded;
            //SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            appMessages.Initialize();
            appMessages.ShowScreenMessage.AddListener(ShowScreen);
            appMessages.BackToPrevScreenMessage.AddListener(BackToPrevScreen);
            appMessages.SetAlertPopupMessage.AddListener(SetActiveAlertPopup);

            activeSceneName = activeScene.name;

            AlertPopup[] alertPopups = GetComponentsInChildren<AlertPopup>(true);
            foreach (var popup in alertPopups)
            {
                popup.Init(this);
                popup.Hide();
            }
            activeAlertPopup = alertPopup;

            foreach (Transform child in transform)
            {
                UIScreen screen = child.GetComponent<UIScreen>();
                if (screen)
                {
                    screensDict.Add(screen.GetType(), screen);
                    screen.gameObject.SetActive(false);
                }
            }

            List<UICanvas> canvasList = SceneUtils.FindObjectsOfType<UICanvas>(true);
            activeCanvas = canvasList.Find(canvas => canvas.ActiveOnAppAwake);
            if (activeCanvas)
            {
                activeCanvas.SetAlertPopup(activeAlertPopup);
                SetPersistentComponentsParent(activeCanvas.TopLayer);
            }

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

        private void OnScenePreload(string sceneName)
        {
            sceneMessageGroups.ForEach(group => group.RemoveAllListeners());

            // Clear screen objects in stack
            foreach (var screenData in screenDataStack)
            {
                if (screenData.Screen)
                    screenData.Screen.ClearCanvasData();
                screenData.Screen = null;
            }
            sceneChangeListener?.OnScenePreload(sceneName);
            sceneLoadingStartTime = Time.time;
        }

        private void OnSceneLoadingProgress(float progress)
        {
        }

        private IEnumerator OnSceneLoadedRoutine(Scene scene)
        {
            if (loadingSceneScreenData != null)
            {
                ScreenData loadedScreenData = loadingSceneScreenData;
                SceneManager.SetActiveScene(scene);
                activeSceneName = scene.name;
                loadingSceneScreenData = null;
                List<UICanvas> canvasList = SceneUtils.FindObjectsOfType<UICanvas>(true);
                foreach (var canvas in canvasList)
                {
                    canvas.Init();
                    canvas.gameObject.SetActive(false);
                }
                void onEnd()
                {
                    ShowScreen(loadedScreenData);
                    screenshotImage.gameObject.SetActive(false);
                    progressSpinnerPanel.Hide();
                    sceneLoadingStartTime = -1f;
                }
                if (sceneChangeListener != null)
                {
                    float sceneLoadingDuration = sceneLoadingStartTime > 0f ? Time.time - sceneLoadingStartTime : 0f;
                    yield return sceneChangeListener?.OnSceneLoadedRoutine(scene, sceneLoadingDuration, onEnd);
                    sceneChangeListener = null;
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
            activeSceneName = newScene.name;
        }

        private IEnumerator OnSceneCloseRoutine(Action onSuccess)
        {
            SetPersistentComponentsParent(transform);
            if (activeCanvas && activeScreenData != null && activeScreenData.Screen)
            {
                activeAlertPopup = alertPopup;
                bool showProgressSpinner = true;
                sceneChangeListener?.OnSceneClose(activeSceneName, out showProgressSpinner);
                activeCanvas.StopAllCoroutines();
                activeScreenData.Screen.gameObject.SetActive(true);
                // Take and show screenshot
                yield return new WaitForEndOfFrame();
                Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
                screenshotImage.sprite = TextureUtils.CreateSpriteFromTexture(texture);
                screenshotImage.gameObject.SetActive(true);
                if (showProgressSpinner)
                {
                    progressSpinnerPanel.Show();
                }
                activeScreenData.Screen.Hide();
                activeCanvas.Hide();
                // Clear data
                activeSceneName = null;
                activeCanvas = null;
                activeScreenData = null;
                // Invoke callback
                onSuccess?.Invoke();
            }
        }

        private void OnSceneClose(Action onSuccess)
        {
            StartCoroutine(OnSceneCloseRoutine(onSuccess));
        }

        private void ShowScreen(ScreenData screenData)
        {
            if (/*SceneUtils.IsLoadingScene || */string.IsNullOrEmpty(activeSceneName))
            {
                return;
            }

            if (activeScreenData != null)
            {
                if (activeScreenData.Screen == screenData.Screen)
                {
                    return;
                }
                else if (activeScreenData.KeepOnStack)
                {
                    screenDataStack.Add(activeScreenData);
                }
                if (activeScreenData.Screen && activeSceneName.Equals(screenData.SceneName))
                {
                    activeScreenData.Screen.Hide();
                }
            }

            if (screenData != null)
            {
                if (screenData.ClearStack)
                {
                    screenDataStack.Clear();
                }
                else
                {
                    int screenIndexInStack = screenDataStack.FindIndex(data => data.ScreenType == screenData.ScreenType);
                    //_screenDataStack.Select(data => data.ScreenType).ToList().Print(".ShowScreen: AppUI.stack: ");
                    if (screenIndexInStack >= 0)
                    {
                        screenDataStack.RemoveRange(screenIndexInStack, screenDataStack.Count - screenIndexInStack);
                    }
                }

                if (screenData.CanvasType == this.GetType())
                {
                    UIScreen screen = screenData.Screen != null ? screenData.Screen : GetScreen(screenData.ScreenType);
                    if (screen)
                    {
                        sceneChangeListener = screen.GetComponent<ISceneChangeListener>();
                        activeCanvas.Hide();
                        screen.ShowInParentCanvas(GetComponent<Canvas>(), activeCanvas);
                    }
                }
                else
                {
                    UIScreen screen = screenData.Screen;
                    //Debug.Log(GetType() + ".ShowScreen: " + screenData.CanvasType + "." + screenData.ScreenType + " " + screen);

                    progressSpinnerPanel.transform.SetParent(transform, false);
                    activeAlertPopup.transform.SetParent(transform, false);

                    if (!activeSceneName.Equals(screenData.SceneName))
                    {
                        //Debug.Log(GetType() + ".ShowScreen: load new scene " + screenData.SceneName + " from " + _activeScreenData.ScreenType);
                        OnSceneClose(() => {
                            loadingSceneScreenData = screenData;
                            SceneUtils.LoadSceneAsync(this, screenData.SceneName, LoadSceneMode.Single, OnScenePreload);
                        });
                        return;
                    }
                    else if (activeCanvas && activeCanvas.GetType() != screenData.CanvasType)
                    {
                        // Hide old canvas
                        activeCanvas.StopAllCoroutines();
                        activeCanvas.SetProgressSpinnerPanel(null);
                        activeCanvas.SetAlertPopup(null);
                        activeCanvas.Hide();
                    }

                    if (screen && screen.Canvas)
                    {
                        activeCanvas = screen.Canvas;
                    }
                    else
                    {
                        activeCanvas = FindCanvasInActiveScene(screenData.CanvasType);
                        SetPersistentComponentsParent(activeCanvas.TopLayer);
                        screen = screen != null ? screen : activeCanvas.GetScreen(screenData.ScreenType);
                        //Debug.Log(GetType() + ".ShowScreen: found screen: " + screen + " " + screen.Canvas);
                        screenData = new ScreenData(screen, screenData.KeepOnStack, screenData.ClearStack);
                    }
                    activeScreenData = screenData;
                    activeCanvas.SetProgressSpinnerPanel(progressSpinnerPanel);
                    activeCanvas.SetAlertPopup(activeAlertPopup);
                    activeCanvas.Show();
                    screen.Show();
                }
            }
        }

        public UIScreen GetScreen(Type screenType)
        {
            if (screensDict.TryGetValue(screenType, out UIScreen screen))
            {
                return screen;
            }
            return null;
        }

        private UICanvas FindCanvasInActiveScene(Type canvasType)
        {
            List<UICanvas> canvasList = SceneUtils.FindObjectsOfType<UICanvas>(true);
            return canvasList.Find(c => c.GetType() == canvasType);
        }

        private void SetAppCanvasRenderMode(RenderMode renderMode)
        {
            Canvas appCanvas = GetComponent<Canvas>();
            if (appCanvas)
            {
                appCanvas.SetRenderMode(renderMode);
            }
        }

        private void SetActiveAlertPopup(Type type)
        {
            if (activeCanvas)
            {
                activeAlertPopup = activeCanvas.TopLayer.GetComponentInChildren(type, true) as AlertPopup;
            }
            if (!activeAlertPopup)
            {
                activeAlertPopup = transform.GetComponentInChildren(type, true) as AlertPopup;
            }
            //Debug.Log(GetType() + ".SetActiveAlertPopup: _activeAlertPopup=" + _activeAlertPopup + " _activeCanvas=" + _activeCanvas);
            if (activeAlertPopup && activeCanvas)
            {
                activeCanvas.SetAlertPopup(activeAlertPopup);
            }
        }

        private void BackToPrevScreen()
        {
            if (screenDataStack.Count > 0)
            {
                if (activeScreenData != null)
                {
                    activeScreenData.KeepOnStack = false;
                }
                ShowScreen(screenDataStack.PickLastElement());
            }
            else
            {
                activeAlertPopup.ShowQuitWarning();
            }
        }

        public void HACKAddScreenDataToScreenStack<T1, T2>(string sceneName) where T1 : UIScreen where T2 : UICanvas
        {
            screenDataStack.Add(new ScreenData(typeof(T1), typeof(T2), sceneName));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (activeAlertPopup.IsShown)
                {
                    activeAlertPopup.OnDismissButtonClick();
                }
                else if (activeScreenData != null && activeScreenData.Screen && activeScreenData.Screen.OnBack())
                {
                    BackToPrevScreen();
                }
            }
        }
    }
}
