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

        public static Vector3 ScreenToWorldTranslation(this Camera camera, Vector2 screenTranslation, float cameraDistance)
        {
            if (camera.orthographic)
            {
                return screenTranslation * camera.orthographicSize * 2f / Screen.height;
            }
            else
            {
                float cameraWorldPlaneHeight = 2f * Mathf.Abs(cameraDistance) * Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2f);
                return screenTranslation * cameraWorldPlaneHeight / Screen.height;
            }
        }

        public static Vector3 ScreenToWorldTranslation(this Camera camera, Vector2 screenTranslation)
        {
            return ScreenToWorldTranslation(camera, screenTranslation, camera.transform.localPosition.z);
        }

        public static bool GetRayIntersectionWithPlane(this Camera camera, Vector3 planeUp, Vector3 planePos, out Vector3 isecPt, out float distance)
        {
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            return Maths.GetRayIntersectionWithPlane(ray, planeUp, planePos, out isecPt, out distance);
        }
    }
}
