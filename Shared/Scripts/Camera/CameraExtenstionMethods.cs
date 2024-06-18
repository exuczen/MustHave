using UnityEngine;
#if UNITY_PIPELINE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif

namespace MustHave
{
    public static partial class CameraExtensionMethods
    {
#if UNITY_PIPELINE_HDRP
        public static HDAdditionalCameraData GetOrAddHDAdditionalCameraData(this Camera camera)
        {
            return camera.GetOrAddComponent<HDAdditionalCameraData>();
        }
#endif
#if UNITY_PIPELINE_URP
        public static void AddUniversalAdditionalCameraData(this Camera camera)
        {
            camera.GetOrAddUniversalAdditionalCameraData();
        }

        public static UniversalAdditionalCameraData GetOrAddUniversalAdditionalCameraData(this Camera camera)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (!cameraData)
            {
                cameraData = camera.GetOrAddComponent<UniversalAdditionalCameraData>();
            }
            return cameraData;
        }
#endif
    }
}
