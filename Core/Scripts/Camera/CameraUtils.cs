using UnityEngine;

namespace MustHave
{
    public struct CameraUtils
    {
        public static Camera MainOrCurrent => Camera.main ? Camera.main : Camera.current;

        public static bool IsBetweenNearAndFar(Camera camera, float z) => z >= camera.nearClipPlane && z <= camera.farClipPlane;
    }
}
