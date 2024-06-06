using UnityEngine;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif

namespace MustHave
{
    public static partial class CameraExtensionMethods
    {
#if UNITY_PIPELINE_URP
        public static void AddUniversalAdditionalCameraData(this Camera camera)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (!cameraData)
            {
                cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
            }
            if (!cameraData)
            {
                camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }
        }

        public static UniversalAdditionalCameraData GetOrAddUniversalAdditionalCameraData(this Camera camera)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (!cameraData)
            {
                cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
            }
            if (!cameraData)
            {
                cameraData = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }
            return cameraData;
        }
#endif
    }
}
