using UnityEngine;

namespace MustHave.Utilities
{
    public static class RectTransformExtensionMethods
    {
        public static void FillParent(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.offsetMax = rectTransform.offsetMin = Vector2.zero;
        }

        /// <summary>
        /// rect transform into coordinates expressed as seen on the screen (in pixels)
        /// takes into account RectTrasform pivots
        /// based on answer by Tobias-Pott
        /// http://answers.unity3d.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="upsideDown"></param>
        /// <returns></returns>
        public static Rect GetScreenSpaceRect(this RectTransform transform, bool upsideDown = false)
        {
            Vector2 size = transform.GetScreenSpaceSize();
            Rect rect;
            if (upsideDown)
            {
                rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
                rect.x -= (transform.pivot.x * size.x);
                rect.y -= ((1.0f - transform.pivot.y) * size.y);
            }
            else
            {
                rect = new Rect(transform.position.x, transform.position.y, size.x, size.y);
                rect.x -= (transform.pivot.x * size.x);
                rect.y -= (transform.pivot.y * size.y);
            }
            return rect;
        }

        public static Vector2 GetScreenSpaceSize(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        }
    }
}
