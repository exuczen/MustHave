using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public struct AssetUtils
    {
        public static void SaveAndRefresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
