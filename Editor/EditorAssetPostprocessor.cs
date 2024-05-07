using System;
using UnityEditor;

namespace MustHave
{
    public class EditorAssetPostprocessor : AssetPostprocessor
    {
        public static event Action AllAssetsPostprocessed = delegate { };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AllAssetsPostprocessed();
        }
    }
}
