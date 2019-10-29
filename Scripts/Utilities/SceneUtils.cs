//#define DEBUG_LOADING_SCENE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MustHave.Utilities
{
    public struct SceneUtils
    {
        public static bool IsLoadingScene { get; private set; }

        public static string ActiveSceneName { get { return SceneManager.GetActiveScene().name; } }

        public static T GetActiveSceneName<T>()
        {
            return ActiveSceneName.ParseToEnum<T>();
        }

        public static Coroutine LoadSceneAsync(MonoBehaviour context, string sceneName, LoadSceneMode mode, Action<string> preLoad = null, Action<float> onProgress = null, Action<Scene> onComplete = null)
        {
            if (!IsLoadingScene && !ActiveSceneName.Equals(sceneName))
            {
                IsLoadingScene = true;
                preLoad?.Invoke(sceneName);
                return context.StartCoroutine(LoadSceneAsyncRoutine(sceneName, mode, onProgress, scene => {
                    onComplete?.Invoke(scene);
                    IsLoadingScene = false;
                }));
            }
            return null;
        }

        private static IEnumerator LoadSceneAsyncRoutine(string sceneName, LoadSceneMode mode, Action<float> onProgress = null, Action<Scene> onComplete = null)
        {
            yield return new WaitForEndOfFrame();
#if DEBUG_LOADING_SCENE
        yield return new WaitForSeconds(1f);
#endif

            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                onProgress?.Invoke(asyncLoad.progress);
                yield return null;
            }
            onProgress?.Invoke(1f);

            onComplete?.Invoke(SceneManager.GetSceneByName(sceneName));
        }

        public static Scene GetDontDestroyOnLoadScene()
        {
            GameObject temp = null;
            try
            {
                temp = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(temp);
                Scene scene = temp.scene;
                UnityEngine.Object.DestroyImmediate(temp);
                temp = null;
                return scene;
            }
            finally
            {
                if (temp != null)
                    UnityEngine.Object.DestroyImmediate(temp);
            }
        }

        public static Canvas FindCanvas(Scene scene, Transform persistentCanvasTransform)
        {
            List<GameObject> rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            Canvas sceneCanvas = null;
            rootGameObjects.Find(root => { return root.transform != persistentCanvasTransform && (sceneCanvas = root.GetComponent<Canvas>()) != null; });
            if (!sceneCanvas)
                rootGameObjects.Find(root => { return (sceneCanvas = root.GetComponentInChildren<Canvas>()) != null; });
            return sceneCanvas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="onlyFirstInRoot">depth first search</param>
        /// <returns></returns>
        public static List<T> FindObjectsOfType<T>(Scene scene, bool onlyFirstInRoot = false) where T : Component
        {
            List<T> results = new List<T>();
            List<GameObject> roots = scene.GetRootGameObjects().ToList();
            if (onlyFirstInRoot)
            {
                foreach (GameObject root in roots)
                {
                    T component = root.GetComponentInChildren<T>(true);
                    if (component)
                        results.Add(component);
                }
            }
            else
            {
                foreach (GameObject root in roots)
                {
                    results.AddRange(root.GetComponentsInChildren<T>(true));
                }
            }
            return results;
        }
    }
}
