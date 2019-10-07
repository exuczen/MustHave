using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MustHave.UI
{
    public interface ISceneChangeListener
    {
        void OnSceneClose(string sceneName, out bool showProgressSpinner);
        IEnumerator OnSceneLoadedRoutine(Scene scene, float loadingDuration, Action onEnd);
        void OnScenePreload(string sceneName);
    }
}
