using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public static class TextureEditor
    {
        [MenuItem("Tools/Create Cube Texture3D")]
        public static void CreateCubeTexture3D()
        {
            // Configure the texture
            int size = 32;
            TextureFormat format = TextureFormat.RGBA32;
            TextureWrapMode wrapMode = TextureWrapMode.Clamp;

            // Create the texture and apply the configuration
            Texture3D texture = new(size, size, size, format, false)
            {
                wrapMode = wrapMode
            };
            // Create a 3-dimensional array to store color data
            Color32[] colors = new Color32[size * size * size];

            // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
            //float inverseResolution = 255f * 1.0f / (size - 1.0f);
            for (int z = 0; z < size; z++)
            {
                int zOffset = z * size * size;
                for (int y = 0; y < size; y++)
                {
                    int yOffset = y * size;
                    for (int x = 0; x < size; x++)
                    {
                        byte r = (byte)(x * 255 / (size - 1));
                        byte g = (byte)(y * 255 / (size - 1));
                        byte b = (byte)(z * 255 / (size - 1));
                        colors[x + yOffset + zOffset] = new Color32(r, g, b, 255);
                    }
                }
            }
            // Copy the color values to the texture
            texture.SetPixels32(colors);

            // Apply the changes to the texture and upload the updated texture to the GPU
            texture.Apply();

            // Save the texture to your Unity Project
            AssetDatabase.CreateAsset(texture, "Assets/CubeTexture3D.asset");
        }

        public static void SliceTextureToSpriteSheet(string texturePath, int width, int height, int colsCount, int rowsCount, bool inverseVertical = false)
        {
            var textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (!textureImporter)
            {
                Debug.Log("SliceTextureToSpriteSheet: file does not exists: " + texturePath);
                return;
            }
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.isReadable = true;
            textureImporter.SaveAndReimport();

            var textureSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureSettings);
            textureImporter.maxTextureSize = Mathf.Max(width, height);
            textureSettings.spritePixelsPerUnit = 100;
            textureSettings.spriteMeshType = SpriteMeshType.Tight;
            textureSettings.spriteExtrude = 0;
            textureImporter.SetTextureSettings(textureSettings);

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(texturePath);
            float spriteWidth = 1f * width / colsCount;
            float spriteHeight = 1f * height / rowsCount;
            var spriteMetas = new List<SpriteMetaData>();
            int rectCount = 0;
            for (int i = 0; i < rowsCount; i++)
            {
                int y = inverseVertical ? rowsCount - i - 1 : i;
                for (int x = 0; x < colsCount; x++)
                {
                    Rect rect = new(x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
                    var meta = new SpriteMetaData
                    {
                        pivot = 0.5f * Vector2.one,
                        alignment = (int)SpriteAlignment.Center,
                        rect = rect,
                        name = filenameWithoutExtension + "_" + rectCount++
                    };
                    spriteMetas.Add(meta);
                }
            }
            textureImporter.spritesheet = spriteMetas.ToArray();
            textureImporter.SaveAndReimport();

            //AssetDatabase.ImportAsset(OutputFilePath, ImportAssetOptions.ForceUpdate);
        }

        public static List<Sprite> GetSprites(string spriteSheetPath)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath).OfType<Sprite>().ToList();

            sprites.Sort((a, b) => {
                var aComps = a.name.Split('_');
                var bComps = b.name.Split('_');
                if (int.TryParse(aComps[aComps.Length - 1], out int aIndex) &&
                    int.TryParse(bComps[bComps.Length - 1], out int bIndex))
                {
                    return aIndex.CompareTo(bIndex);
                }
                return 0;
            });
            return sprites;
        }
    }
}
