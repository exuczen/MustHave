#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MustHave
{
    [ExecuteInEditMode]
    public class UpdateInEditMode : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnEnable()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.update += OnEditorUpdate;
            }
        }

        private void OnDisable()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        private void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
#endif
    }
}
