using UnityEngine;

namespace MustHave
{
    public static class VectorExtensionMethods
    {
        public static Vector2Int Negative(this Vector2Int v)
        {
            return new Vector2Int(-v.x, -v.y);
        }

        public static int GetXYSize(this Vector2Int v)
        {
            return v.x * v.y;
        }

        public static Vector3 Modulo(this Vector3 v, float modulo)
        {
            return new Vector3(
                v.x % modulo,
                v.y % modulo,
                v.z % modulo
                );
        }

        public static bool IsInCameraViewXY(this Vector3 pos, Camera camera, float viewportOffsetX = 0f, float viewportOffsetY = 0f)
        {
            return IsInCameraViewXY((Vector2)pos, camera, viewportOffsetX, viewportOffsetY);
        }

        public static bool IsInCameraViewXY(this Vector2 pos, Camera camera, float viewportOffsetX = 0f, float viewportOffsetY = 0f)
        {
            Vector2 viewportPos = camera.WorldToViewportPoint(pos);
            return
                viewportPos.x >= -viewportOffsetX && viewportPos.x <= 1f + viewportOffsetX &&
                viewportPos.y >= -viewportOffsetY && viewportPos.y <= 1f + viewportOffsetY;
        }
    }
}
