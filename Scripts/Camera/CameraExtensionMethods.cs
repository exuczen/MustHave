using UnityEngine;

namespace MustHave
{
    public static class CameraExtensionMethods
    {
        public static Bounds2 GetOrthographicBounds2WithOffset(this Camera camera, float viewportOffset)
        {
            float offset = viewportOffset * camera.orthographicSize;
            return new Bounds2(camera.transform.position,
                new Vector2(camera.orthographicSize * Screen.width / Screen.height + offset, camera.orthographicSize + offset) * 2f);
        }

        public static Bounds2 GetOrthographicBounds2(this Camera camera)
        {
            return new Bounds2(camera.transform.position,
                new Vector2(camera.orthographicSize * Screen.width / Screen.height, camera.orthographicSize) * 2f);
        }

        public static float GetTanHalfFovFromProjectionMatrix(this Camera camera)
        {
            return camera.projectionMatrix.GetTanHalfFovFromProjection();
        }

        public static float GetFovFromProjectionMatrix(this Camera camera)
        {
            return camera.projectionMatrix.GetFovFromProjection();
        }

        public static Vector3 ScreenToWorldTranslation(this Camera camera, Vector2 screenDeltaPos)
        {
            if (camera.orthographic)
            {
                return screenDeltaPos * camera.orthographicSize * 2f / Screen.height;
            }
            else
            {
                float cameraDistance = camera.transform.localPosition.z;
                Vector3 screenPoint = new Vector3(Screen.width / 2f + screenDeltaPos.x, Screen.height / 2f + screenDeltaPos.y, -cameraDistance);
                Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
                Vector3 cameraPlaneWorldPoint = camera.transform.TransformPoint(new Vector3(0f, 0f, -cameraDistance));
                return worldPoint - cameraPlaneWorldPoint;
            }
        }
    }
}
