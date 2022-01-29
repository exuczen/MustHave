using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MustHave.UI;
using MustHave.Utils;

namespace MustHave.IconsRenderer
{
    public class IconsRenderer : MonoBehaviour
    {
        public int SheetTextureSize => cellXYCount * iconSize;
        public int CellXYCount => cellXYCount;
        public string SpriteSheetFilePath => @spriteSheetFolderPath + "/" + spriteSheetFileName;

        [SerializeField, RequireInterface(typeof(IIconSourceProvider))]
        private Object iconSourceProvider = null;
        [SerializeField]
        private Transform sourceObjectsContainer = null;
        [SerializeField]
        private Transform cameraPivot = null;
        [SerializeField]
        private Camera sourceCamera = null;
        [SerializeField]
        private Camera sheetCamera = null;
        [SerializeField]
        private GameObject lights = null;
        [SerializeField]
        private MeshRenderer sourceQuad = null;
        [SerializeField]
        private string spriteSheetFolderPath = @"Assets/";
        [SerializeField]
        private string spriteSheetFileName = @"Icons.png";
        [SerializeField]
        private int cellXYCount = 8;
        [SerializeField]
        private int iconSize = 128;
        [SerializeField]
        private float iconNormalizedOffset = 0.1f;
        [SerializeField]
        private List<IconSourceObject> sourceObjects = new List<IconSourceObject>();

        private void Start()
        {
            RenderIcons();
        }

        public void RecreateSourceObjects()
        {
            if (iconSourceProvider == null)
            {
                return;
            }
            DestroySourceObjects();

            IIconSourceProvider provider = iconSourceProvider as IIconSourceProvider;
            for (int i = 0; i < provider.IconSourceCount; i++)
            {
                var source = provider.GetIconSourcePrefab(i).CreateInstance(sourceObjectsContainer);
                source.SetGameObjectActive(false);
                sourceObjects.Add(source);
            }
        }

        public void DestroySourceObjects()
        {
            if (Application.isPlaying)
            {
                sourceObjectsContainer.DestroyAllChildren();
            }
            else
            {
                sourceObjectsContainer.DestroyAllChildrenImmediate();
            }
            sourceObjects.Clear();
        }

        public void RenderIcons()
        {
            if (sourceObjects.Count == 0)
            {
                return;
            }
            RecreateRenderTextures();

            lights.SetActive(true);
            sourceObjectsContainer.SetGameObjectActive(true);

            sourceCamera.SetGameObjectActive(true);
            sourceCamera.enabled = false;

            sheetCamera.SetGameObjectActive(true);
            sheetCamera.orthographic = true;
            sheetCamera.orthographicSize = CellXYCount * 0.5f;
            sheetCamera.enabled = false;

            sheetCamera.clearFlags = CameraClearFlags.SolidColor;
            sheetCamera.backgroundColor = Color.clear;
            sheetCamera.Render();
            sheetCamera.clearFlags = CameraClearFlags.Depth;

            sourceQuad.transform.localScale = Vector3.one;

            float scale = 1f;
            float offset = (CellXYCount * 0.5f - 0.5f) * scale;
            //float iconsSize = 1f / CellXYCount;

            for (int i = 0; i < sourceObjects.Count; i++)
            {
                var source = sourceObjects[i];
                source.SetGameObjectActive(true);
                source.OnPreRender();
                int x = i % cellXYCount;
                int y = i / cellXYCount;
                sourceQuad.transform.localPosition = new Vector3(-offset + x * scale, -offset + y * scale, 5f);

                sourceCamera.SetupViewForRender(source.BoxCollider, cameraPivot, iconNormalizedOffset);
                sourceCamera.Render();
                sheetCamera.Render();

                source.SetGameObjectActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="antialiasing">This is home-made antialiasing.</param>
        public void SaveTextureToPNG(bool antialiasing)
        {
            if (sheetCamera.targetTexture)
            {
                var texture = sheetCamera.targetTexture.ToTexture2D(TextureFormat.ARGB32, false);
                if (antialiasing)
                {
                    TextureUtils.SetPixelsAlphaFromClosestPixels(texture, 20);
                }
                TextureUtils.SaveTextureToPNG(texture, @spriteSheetFolderPath, spriteSheetFileName);
            }
        }

#if UNITY_EDITOR
        public void AssignIconsToPrefabs(List<Sprite> sprites)
        {
            IIconSourceProvider provider = iconSourceProvider as IIconSourceProvider;
            int count = Mathf.Min(sprites.Count, provider.IconSourceCount);
            for (int i = 0; i < count; i++)
            {
                var iconSourcePrefab = provider.GetIconSourcePrefab(i);
                iconSourcePrefab.Sprite = sprites[i];
                EditorUtility.SetDirty(iconSourcePrefab);
                //Debug.Log("AssignIconsToPrefabs: " + sprites[i].name + " " + iconSource.IconSourceGameObject.name);
            }
            if (count > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif

        private void RecreateRenderTextures()
        {
            if (Application.isPlaying)
            {
                DestroyRenderTexture(sourceCamera);
                DestroyRenderTexture(sheetCamera);
            }
            Material quadMaterial = Application.isPlaying ? sourceQuad.material : sourceQuad.sharedMaterial;
            sourceCamera.targetTexture = RecreateTexture(sourceCamera.targetTexture, iconSize, iconSize, "QuadRenderTexture");
            quadMaterial.mainTexture = sourceCamera.targetTexture;

            sheetCamera.targetTexture = RecreateTexture(sheetCamera.targetTexture, SheetTextureSize, SheetTextureSize, "SheetRenderTexture");
        }

        private void DestroyRenderTexture(Camera camera)
        {
            DestroyRenderTexture(camera.targetTexture);
            camera.targetTexture = null;
        }

        private void DestroyRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture)
            {
                if (Application.isPlaying)
                {
                    Destroy(renderTexture);
                }
                else
                {
                    DestroyImmediate(renderTexture);
                }
            }
        }

        private RenderTexture RecreateTexture(RenderTexture renderTexture, int width, int height, string name)
        {
            if (!renderTexture || renderTexture.width != width || renderTexture.height != height)
            {
                if (renderTexture)
                {
                    DestroyRenderTexture(renderTexture);
                }
                renderTexture = new RenderTexture(width, height, 0) {
                    antiAliasing = 8,
                };
            }
            renderTexture.name = name;

            return renderTexture;
        }
    }
}