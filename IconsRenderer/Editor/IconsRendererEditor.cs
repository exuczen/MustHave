using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(IconsRenderer))]
    public class IconsRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            IconsRenderer renderer = target as IconsRenderer;

            GUILayout.Space(20f);
            if (GUILayout.Button("Recreate Source Objects"))
            {
                renderer.RecreateSourceObjects();
            }
            if (GUILayout.Button("Render Icons"))
            {
                renderer.RenderIcons();
            }
            if (GUILayout.Button("Save To PNG"))
            {
                SaveTextureToPNG(renderer);
            }
            if (GUILayout.Button("Save To Sprite Sheet"))
            {
                SaveTextureToSpriteSheet(renderer);
            }
            if (GUILayout.Button("Assign Icons To Prefabs"))
            {
                AssignIconsToPrefabs(renderer);
            }
            if (GUILayout.Button("Destroy Source Objects"))
            {
                renderer.DestroySourceObjects();
            }
        }

        private void SaveTextureToPNG(IconsRenderer renderer)
        {
            renderer.SaveTextureToPNG(true);
            AssetDatabase.Refresh();
        }

        private void SaveTextureToSpriteSheet(IconsRenderer renderer)
        {
            SaveTextureToPNG(renderer);
            int xyCount = renderer.CellXYCount;
            int size = renderer.SheetTextureSize;
            TextureEditor.SliceTextureToSpriteSheet(renderer.SpriteSheetFilePath, size, size, xyCount, xyCount);
        }

        private void AssignIconsToPrefabs(IconsRenderer renderer)
        {
            var sprites = TextureEditor.GetSprites(renderer.SpriteSheetFilePath);
            renderer.AssignIconsToPrefabs(sprites);
        }
    }
}