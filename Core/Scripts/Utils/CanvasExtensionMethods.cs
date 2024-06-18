using UnityEngine;

namespace MustHave
{
    public static class CanvasExtensionMethods
    {
        public static void SetRenderMode(this Canvas canvas, RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                case RenderMode.WorldSpace:
                    canvas.worldCamera = null;
                    break;
                case RenderMode.ScreenSpaceCamera:
                    canvas.worldCamera = CameraUtils.MainOrCurrent;
                    break;
                default:
                    break;
            }
            canvas.renderMode = renderMode;
        }
    } 
}
