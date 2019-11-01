using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MustHave.Utilities
{
    public struct TextureUtils
    {
        /// <summary>
        /// Creates the texture filled with given color.
        /// </summary>
        public static Texture2D CreateTexture(int width, int height, Color fillColor)
        {
            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result.SetPixel(x, y, fillColor);
                }
            }
            result.Apply();
            return result;
        }

        /// <summary>
        /// Converts whole texture to one sprite.
        /// </summary>
        public static Sprite CreateSpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static void LoadImageFromFilepath(string filePath, Image image)
        {
            Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
            var bytes = File.ReadAllBytes(filePath);
            texture.LoadImage(bytes);
            Sprite sprite = CreateSpriteFromTexture(texture);
            if (image)
            {
                image.sprite = sprite;
            }
        }

        public static IEnumerator CaptureScreenshotWithoutCanvasRoutine(Camera camera, Canvas canvas, System.Action<Texture2D> resultCallback)
        {
            bool canvasEnabled = canvas.enabled;
            canvas.enabled = false;
            yield return new WaitForEndOfFrame();
            Texture2D texture = CaptureScreenshotByRenderingToTexture(camera, TextureFormat.ARGB32);
            canvas.enabled = canvasEnabled;
            if (resultCallback != null)
                resultCallback.Invoke(texture);
        }

        public static Texture2D CaptureScreenshotByRenderingToTexture(Camera camera, TextureFormat textureFormat)
        {
            int width = Screen.width;
            int height = Screen.height;
            return CaptureScreenshotByRenderingToTexture(camera, textureFormat, width, height);
        }

        /// <summary>
        /// /// <summary>Take screenshot using RenderTexture and Camera</summary>
        /// </summary>
        /// <param name="resultCallback"></param>
        /// <returns></returns>
        public static Texture2D CaptureScreenshotByRenderingToTexture(Camera camera, TextureFormat textureFormat, int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24/*, RenderTextureFormat.ARGB32*/);
            // TODO: Customize anti-aliasing value. Anti-aliasing value must be one of (1, 2, 4 or 8), indicating the number of samples per pixel.
            renderTexture.antiAliasing = 4;
            RenderTexture activePreviously = RenderTexture.active;
            RenderTexture.active = renderTexture;
            RenderTexture target = camera.targetTexture;
            camera.targetTexture = renderTexture;
            camera.Render();
            Texture2D texture = new Texture2D(width, height, textureFormat, false);
            // Read screen contents into the texture
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            RenderTexture.active = activePreviously;
            camera.targetTexture = target;
            UnityEngine.Object.Destroy(renderTexture);
            texture.name = "ScreenshotTexture";
            return texture;
        }

        /// <summary>
        /// WARNING: ScreenCapture.CaptureScreenshotAsTexture does not work on iOS with OpenGLES, it works only with metal
        /// </summary>
        /// <param name="resultCallback"></param>
        /// <returns></returns>
        public static IEnumerator CaptureScreenshotAsTextureRoutine(System.Action<Texture2D> resultCallback)
        {
            yield return new WaitForEndOfFrame();
            Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
            if (resultCallback != null)
                resultCallback.Invoke(texture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="spriteName"></param>
        public static void CaptureScreenshotToImage(Camera camera, Image image, string spriteName = null)
        {
            if (image.sprite)
                UnityEngine.Object.Destroy(image.sprite);
            image.sprite = CreateSpriteFromTexture(CaptureScreenshotByRenderingToTexture(camera, TextureFormat.RGB24));
            if (!string.IsNullOrEmpty(spriteName))
                image.sprite.name = spriteName;
        }

        public static void CaptureScreenshotToImage(Image image, string spriteName = null)
        {
            CaptureScreenshotToImage(CameraUtils.MainOrCurrent, image, spriteName);
        }
    } 
}
