using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MustHave
{
#if UNITY_EDITOR
    public class UnityAssetPostprocessor : AssetPostprocessor
#else
    public class UnityAssetPostprocessor
#endif
    {
        public static event Action AllAssetsPostprocessed = delegate { };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AllAssetsPostprocessed();
        }
    }
}
