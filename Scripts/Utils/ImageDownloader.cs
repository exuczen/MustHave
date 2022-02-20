using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MustHave.Utils
{
    public struct ImageDownloader
    {
        private const int NETWORK_CONNECTION_DOWNLOAD_TIMEOUT = 20;

        /// <summary>
        /// Downloads image from url into texture. Texture should not be null (after download it's data will be overriden).
        /// To access cross-domain WWW resources in WebGL, the server you are trying to access needs to authorize this using CORS.
        /// https://docs.unity3d.com/Manual/webgl-networking.html
        /// </summary>
        public static Coroutine DownloadInto(MonoBehaviour context, string url, Action<Texture2D> onSuccess)
        {
            if (string.IsNullOrEmpty(url) || onSuccess == null || context == null)
                return null;

            return context.StartCoroutine(DownloadIntoRoutine(url, onSuccess));
        }

        /// <summary>
        /// Download texture with callback.
        /// </summary>
        public static Coroutine DownloadInto(MonoBehaviour context, string url, Action<string, Texture2D> resultCallback)
        {
            if (string.IsNullOrEmpty(url) || resultCallback == null || context == null)
                return null;

            return context.StartCoroutine(DownloadIntoRoutine(url, resultCallback));
        }

        /// <summary>
        /// Download sprite with callback.
        /// </summary>
        public static Coroutine DownloadInto(MonoBehaviour context, string url, Action<string, Texture2D, Sprite> resultCallback)
        {
            if (string.IsNullOrEmpty(url) || resultCallback == null || context == null)
                return null;

            return context.StartCoroutine(DownloadIntoRoutine(url, resultCallback));
        }

        /// <summary>
        /// Downloads image from url into image as a sprite.
        /// To access cross-domain WWW resources in WebGL, the server you are trying to access needs to authorize this using CORS.
        /// https://docs.unity3d.com/Manual/webgl-networking.html
        /// </summary>
        public static Coroutine DownloadInto(MonoBehaviour context, string url, Image image, Color color)
        {
            if (string.IsNullOrEmpty(url) || image == null || context == null)
                return null;

            return context.StartCoroutine(DownloadIntoRoutine(url, image, color));
        }

        /// <summary>
        /// Downloads image from url or loads it from Application.persistentDataPath/folderPath/[stripped url] if exists.
        /// </summary>
        public static Coroutine DownloadIntoOrLoadFromFolder(string folderPath, MonoBehaviour context, string url, Image image, Action onSuccess = null, Action<string> onError = null)
        {
            if (string.IsNullOrEmpty(url) || image == null || context == null)
                return null;

            string dirPath = Path.Combine(Application.persistentDataPath, folderPath);
            return context.StartCoroutine(DownloadIntoOrLoadFromFolderRoutine(url, image, dirPath, onSuccess, onError));
        }

        public static Coroutine DownloadIntoOrLoadFromPicturesFolder(MonoBehaviour context, string url, Image image, Action onSuccess = null, Action<string> onError = null)
        {
            return DownloadIntoOrLoadFromFolder("Pictures", context, url, image, onSuccess, onError);
        }

        public static Coroutine DownloadIntoOrLoadFromAvatarsFolder(MonoBehaviour context, string url, Image image, Action onSuccess = null, Action<string> onError = null)
        {
            return DownloadIntoOrLoadFromFolder("Avatars", context, url, image, onSuccess, onError);
        }

        /// <summary>
        /// Download texture routine.
        /// </summary>
        private static IEnumerator DownloadIntoRoutine(string url, Action<Texture2D> onSuccess, Action<string> onError = null)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                www.timeout = NETWORK_CONNECTION_DOWNLOAD_TIMEOUT;
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    onSuccess(DownloadHandlerTexture.GetContent(www));
                }
                else
                {
                    Debug.LogWarning("ImageUtils.DownloadIntoRoutine: " + www.error);
                    onError?.Invoke(www.error);
                }
            }
        }

        /// <summary>
        /// Download texture with callback routine.
        /// </summary>
        private static IEnumerator DownloadIntoRoutine(string url, Action<string, Texture2D> resultCallback)
        {
            yield return DownloadIntoRoutine(url, texture => {
                resultCallback.Invoke(url, texture);
            });
        }

        /// <summary>
        /// Download sprite with callback routine.
        /// </summary>
        private static IEnumerator DownloadIntoRoutine(string url, Action<string, Texture2D, Sprite> resultCallback)
        {
            yield return DownloadIntoRoutine(url, texture => {
                Sprite sprite = TextureUtils.CreateSpriteFromTexture(texture);
                resultCallback.Invoke(url, texture, sprite);
            });
        }

        /// <summary>
        /// Download texture into image routine.
        /// </summary>
        private static IEnumerator DownloadIntoRoutine(string url, Image image, Color color)
        {
            yield return DownloadIntoRoutine(url, texture => {
                Sprite sprite = TextureUtils.CreateSpriteFromTexture(texture);
                image.sprite = sprite;
                image.color = color;
            });
        }

        public static string GetImageFileNameFromUrl(string url, string extension = null)
        {
            string clearName = url;
            clearName = clearName.Replace("/", "");
            clearName = clearName.Replace(":", "");
            clearName = clearName.Replace("=", "");
            clearName = clearName.Replace("?", "");
            return !string.IsNullOrEmpty(extension) ? string.Concat(clearName, ".", extension) : clearName;
        }

        public static string GetImageFilePathFromUrl(string url, string path, string extension = null)
        {
            string fileName = GetImageFileNameFromUrl(url, extension);
            return Path.Combine(path, fileName);
        }

        private static IEnumerator DownloadIntoOrLoadFromFolderRoutine(string url, Image image, string dirPath, Action onSuccess, Action<string> onError)
        {
            yield return new WaitForEndOfFrame();

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string filePath = GetImageFilePathFromUrl(url, dirPath);
            if (!File.Exists(filePath))
            {
                yield return DownloadIntoRoutine(url,
                    texture => {
                        Sprite sprite = TextureUtils.CreateSpriteFromTexture(texture);
                        if (image)
                        {
                            image.sprite = sprite;
                        }
                        byte[] bytes;
                        string extension = Path.GetExtension(url);
                        if (string.Compare(extension, ".jpeg", true) == 0 || string.Compare(extension, ".jpg", true) == 0)
                        {
                            bytes = texture.EncodeToJPG();
                        }
                        else
                        {
                            bytes = texture.EncodeToPNG();
                        }
                        File.WriteAllBytes(filePath, bytes);
                        onSuccess?.Invoke();
                    },
                    error => {
                        Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                        Sprite sprite = TextureUtils.CreateSpriteFromTexture(texture);
                        if (image)
                        {
                            image.sprite = sprite;
                        }
                        onError?.Invoke(url);
                    });
            }
            else
            {
                TextureUtils.LoadImageFromFilepath(filePath, image);
                onSuccess?.Invoke();
            }
        }
    }
}
