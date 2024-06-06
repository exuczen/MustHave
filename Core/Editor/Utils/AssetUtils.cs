using System;
using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public struct AssetUtils
    {
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

        public static void SaveAndRefresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
