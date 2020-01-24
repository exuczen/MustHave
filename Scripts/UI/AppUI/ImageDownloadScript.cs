//#define WEBP

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MustHave.Utilities;
#if WEBP
using WebP;
#endif

namespace MustHave.UI
{
    public enum ImageDownloadFormat
    {
        PNG_JPG,
#if WEBP
        WebP
#endif
    }

    [RequireComponent(typeof(Image))]
    public class ImageDownloadScript : MonoBehaviour
    {
        [SerializeField] private Animator _progressSpinner = default;

        private Image _image = default;
        private CanvasScript _canvas = default;

        public Image Image => _image ?? (_image = GetComponent<Image>());
        private CanvasScript Canvas => _canvas ?? (_canvas = transform.GetComponentInParents<CanvasScript>());
        public RectTransform RectTransform => transform as RectTransform;

        public void DownloadOrLoadImage(ImageDownloadFormat format, string imageURL, string appDataFolderName, float loadingColorAlpha, Action onSuccess = null)
        {
            if (!string.IsNullOrEmpty(imageURL) && !string.IsNullOrEmpty(appDataFolderName))
            {
                _progressSpinner.gameObject.SetActive(true);
                Image image = Image;
                image.sprite = null;
                image.color = ColorUtils.ColorWithAlpha(image.color, loadingColorAlpha);
                void onEnd()
                {
                    image.color = ColorUtils.ColorWithAlpha(image.color, 1f);
                    _progressSpinner.gameObject.SetActive(false);
                    onSuccess?.Invoke();
                }
                switch (format)
                {
                    case ImageDownloadFormat.PNG_JPG:
                        ImageDownloader.DownloadIntoOrLoadFromFolder(appDataFolderName, Canvas, imageURL, image, onEnd);
                        break;
#if WEBP
                    case ImageDownloadFormat.WebP:
                        DownloadIntoOrLoadWebPFromFolder(appDataFolderName, Canvas, imageURL, image, onEnd);
                        break;
#endif
                    default:
                        break;
                }
            }
        }
#if WEBP
        private void DownloadIntoOrLoadWebPFromFolder(string appDataFolderName, MonoBehaviour context, string url, Image image, Action onSuccess = null, Action<string> onError = null)
        {
            string folderPath = Path.Combine(Application.persistentDataPath, appDataFolderName);
            context.StartCoroutine(DownloadIntoOrLoadWebPFromFolderRoutine(folderPath, context, url, image, onSuccess, onError));
        }

        private IEnumerator DownloadIntoOrLoadWebPFromFolderRoutine(string folderPath, MonoBehaviour context, string url, Image image, Action onSuccess = null, Action<string> onError = null)
        {
            yield return new WaitForEndOfFrame();

            string filePath = ImageDownloader.GetImageFilePathFromUrl(url, folderPath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            if (File.Exists(filePath))
            {
                LoadTextureFromWebP(File.ReadAllBytes(filePath));
                onSuccess?.Invoke();
            }
            else
            {
                WWWUtils.LoadBinaryFromWWW(this, url, bytes => {
                    LoadTextureFromWebP(bytes);
                    File.WriteAllBytes(filePath, bytes);
                    onSuccess?.Invoke();
                });
            }
        }

        private void LoadTextureFromWebP(byte[] bytes)
        {
            Texture2D texture = Texture2DExt.CreateTexture2DFromWebP(bytes, lMipmaps: true, lLinear: true, lError: out Error lError);
            if (lError == Error.Success)
                _image.sprite = TextureUtils.CreateSpriteFromTexture(texture);
            else
                Debug.LogError(GetType() + ".LoadTextureFromWebP: WebP Load Error : " + lError.ToString());
        }
#endif
    }
}
