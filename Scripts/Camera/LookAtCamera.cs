using UnityEngine;

namespace MustHave
{
    public class LookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            Camera camera = CameraUtils.MainOrCurrent;
            transform.rotation = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
        }
    }
}
