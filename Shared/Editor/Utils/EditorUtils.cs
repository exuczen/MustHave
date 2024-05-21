using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine.SceneManagement;

namespace MustHave
{
    public class EditorUtils
    {
        public static void SaveActiveScene()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        public static bool SetDirtyOnEndChangeCheck(UnityEngine.Object target)
        {
            if (EditorGUI.EndChangeCheck())
            {
                SetSceneOrObjectDirty(target);
                return true;
            }
            return false;
        }

        public static void SetSceneOrObjectDirty(UnityEngine.Object target)
        {
            if (target)
            {
                EditorUtility.SetDirty(target);
            }
            else
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        public static void SetSceneOrPrefabDirty(UnityEngine.Object target)
        {
            if (target && PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                EditorUtility.SetDirty(target);
            }
            else
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}
