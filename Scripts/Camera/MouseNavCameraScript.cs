using UnityEngine;

namespace MustHave
{
    [RequireComponent(typeof(Camera))]
    public class MouseNavCameraScript : MonoBehaviour
    {
        [SerializeField] private float translationSpeed = default;
        [SerializeField] private float zoomSpeed = default;

        private const float ROTATION_RATE = 240f;

        private new Camera camera = default;
        private Vector3 mousePositionPrev = default;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            mousePositionPrev = Input.mousePosition;
        }

        private void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseDeltaPos = mousePosition - mousePositionPrev;
            mousePositionPrev = mousePosition;

            if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
            {
                Vector3 translation = -translationSpeed * camera.ScreenToWorldTranslation(mouseDeltaPos);
                transform.Translate(translation, Space.Self);
            }
            else if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                if (Input.GetKey(KeyCode.LeftAlt) &&
                    Maths.GetRayIntersectionWithPlane(transform.position, transform.forward, Vector3.up, Vector3.zero, out Vector3 rotationPivot, out _))
                {
                    transform.RotateAround(rotationPivot, transform.right, -mouseY * ROTATION_RATE * Time.deltaTime);
                    transform.RotateAround(rotationPivot, Vector3.up, mouseX * ROTATION_RATE * Time.deltaTime);
                }
            }
            Vector2 mouseScrollDelta = Input.mouseScrollDelta;
            if (mouseScrollDelta.y != 0f)
            {
                if (camera.orthographic)
                {
                    camera.orthographicSize -= zoomSpeed * Time.deltaTime * mouseScrollDelta.y;
                }
                else
                {
                    transform.Translate(0f, 0f, zoomSpeed * Time.deltaTime * mouseScrollDelta.y);
                }
            }
        }
    }
}
