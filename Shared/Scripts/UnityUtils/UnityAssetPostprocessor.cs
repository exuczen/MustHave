#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace MustHave
{
#if UNITY_EDITOR
    public class UnityAssetPostprocessor : AssetPostprocessor
    {
        public static event Action AllAssetsPostprocessed = delegate { };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AllAssetsPostprocessed();
        }
    }
#else
    public class UnityAssetPostprocessor {}
#endif
}
