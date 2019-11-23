using MustHave.Utilities;
using UnityEngine;

namespace MustHave.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class MouseNavCameraScript : MonoBehaviour
    {
        [SerializeField]
        private float _translationSpeed = default;
        [SerializeField]
        private float _zoomSpeed = default;

        private const float ROTATION_RATE = 240f;

        private Camera _camera = default;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (Input.GetMouseButton(2))
            {
                Vector3 translation = _translationSpeed * Time.deltaTime * new Vector3(-mouseX, -mouseY, 0f);
                transform.Translate(translation, Space.Self);
            }
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0))
            {
                if (Maths.GetRayIntersectionWithPlane(transform.position, transform.forward, Vector3.up, Vector3.zero, out Vector3 rotationPivot))
                {
                    transform.RotateAround(rotationPivot, transform.right, -mouseY * ROTATION_RATE * Time.deltaTime);
                    transform.RotateAround(rotationPivot, Vector3.up, mouseX * ROTATION_RATE * Time.deltaTime);
                }
            }
            Vector2 mouseScrollDelta = Input.mouseScrollDelta;
            if (mouseScrollDelta.y != 0f)
            {
                if (_camera.orthographic)
                {
                    _camera.orthographicSize -= _zoomSpeed * Time.deltaTime * mouseScrollDelta.y;
                }
                else
                {
                    transform.Translate(0f, 0f, _zoomSpeed * Time.deltaTime * mouseScrollDelta.y);
                }
            }
        }
    }
}
