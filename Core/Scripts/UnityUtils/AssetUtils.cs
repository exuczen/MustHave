#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public struct AssetUtils
    {
        public static void ModifyPrefab<T>(Action<T> modify, string prefabPath) where T : MonoBehaviour
        {
            ModifyPrefab(gameObject => {
                if (gameObject.TryGetComponent<T>(out var component))
                {
                    modify(component);
                }
            }, prefabPath);
        }

        public static void ModifyPrefab(Action<GameObject> modify, string prefabPath)
        {
            var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            if (!prefab)
            {
                return;
            }
            modify(prefab);
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefab);
            SaveAndRefresh();
        }

        public static void SaveAsset<T>(T target) where T : UnityEngine.Object
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        }

        public static void SaveAndRefresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif