#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
#endif

namespace MustHave
{
#if UNITY_EDITOR
    public struct EditorUtils
    {
        public static Action<bool> UnityEditorFocusChanged
        {
            get
            {
                var fieldInfo = typeof(EditorApplication).GetField("focusChanged", BindingFlags.Static | BindingFlags.NonPublic);
                return (Action<bool>)fieldInfo.GetValue(null);
            }
            set
            {
                var fieldInfo = typeof(EditorApplication).GetField("focusChanged", BindingFlags.Static | BindingFlags.NonPublic);
                fieldInfo.SetValue(null, value);
            }
        }

        public static bool AddLayer(string layerName, out bool exists)
        {
            //for (int i = 0; i < InternalEditorUtility.layers.Length; i++)
            //{
            //    Debug.Log($"AddLayer: Layer[{i}]: {InternalEditorUtility.layers[i]}");
            //}
            var tagManagerObject = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            var tagManager = new SerializedObject(tagManagerObject);
            var layers = tagManager.FindProperty("layers");

            int firstEmptySlotIndex = -1;

            exists = false;

            for (int i = 0; i < layers.arraySize; i++)
            {
                var element = layers.GetArrayElementAtIndex(i);
                if (!string.IsNullOrEmpty(element.stringValue))
                {
                    if (layerName.Equals(element.stringValue))
                    {
                        Debug.LogWarning($"AddLayer: Layer {layerName} exists at index {i}.");
                        exists = true;
                        return false;
                    }
                }
                else if (firstEmptySlotIndex < 0)
                {
                    firstEmptySlotIndex = i;
                }
            }
            if (firstEmptySlotIndex < 0)
            {
                Debug.LogError($"AddLayer: Can't add {layerName}. All slots are used.");

                return false;
            }
            else
            {
                var firstEmptySlot = layers.GetArrayElementAtIndex(firstEmptySlotIndex);
                firstEmptySlot.stringValue = layerName;

                tagManager.ApplyModifiedProperties();
                tagManager.Update();

                AssetDatabase.Refresh();

                Debug.Log($"AddLayer: Added {layerName} at index {firstEmptySlotIndex}");

                return true;
            }
        }

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
#else
    public struct EditorUtils {}
#endif
}
