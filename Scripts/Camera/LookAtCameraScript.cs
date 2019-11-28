using UnityEngine;

namespace MustHave
{
    public class LookAtCameraScript : MonoBehaviour
    {
        private void Update()
        {
            Camera camera = CameraUtils.MainOrCurrent;
            transform.rotation = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
        }
    }
}
