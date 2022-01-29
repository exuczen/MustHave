﻿//#define DEBUG_LOADING_SCENE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MustHave.Utils
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
            yield return null;
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

        public static Canvas FindCanvas(Transform excludeCanvasTransform)
        {
            List<GameObject> rootGameObjects = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootGameObjects);
            Canvas sceneCanvas = null;
            rootGameObjects.Find(root => root.transform != excludeCanvasTransform && (sceneCanvas = root.GetComponent<Canvas>()) != null);
            if (!sceneCanvas)
                rootGameObjects.Find(root => (sceneCanvas = root.GetComponentInChildren<Canvas>()) != null);
            return sceneCanvas;
        }

        public static T FindObjectOfType<T>() where T : Component
        {
            Scene scene = SceneManager.GetActiveScene();
            List<GameObject> roots = scene.GetRootGameObjects().ToList();
            foreach (GameObject root in roots)
            {
                T component = root.GetComponentInChildren<T>(true);
                if (component)
                {
                    return component;
                }
            }
            return null;
        }

        public static List<T> FindObjectsOfType<T>(bool firstDepthSearch = false) where T : Component
        {
            List<T> results = new List<T>();
            Scene scene = SceneManager.GetActiveScene();
            List<GameObject> roots = scene.GetRootGameObjects().ToList();
            if (firstDepthSearch)
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

        public static T FindRootObjectOfType<T>() where T : Component
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                T component = root.GetComponent<T>();
                if (component)
                    return component;
            }
            return null;
        }
    }
}
