using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.SceneManagement;

namespace MustHave
{
    public struct EditorUtils
    {
        public static string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            var sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
            return sortingLayers;
        }

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
