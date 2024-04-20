using UnityEngine;

namespace MustHave
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class MouseNavCameraScript : MonoBehaviour
    {
        [SerializeField] private float translationSpeed = 1f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float rotationSpeed = 4f;
        [SerializeField] private float distance = 10f;
        [SerializeField] private Transform target = null;
        [SerializeField] private Transform targetPlane = null;
        [SerializeField, HideInInspector] private new Camera camera = default;

        private Vector3 mousePositionPrev = default;

        private void OnEnable()
        {
            camera = GetComponent<Camera>();
            mousePositionPrev = Input.mousePosition;

            if (target)
            {
                UpdateCameraPosition();
            }
        }

        private void OnValidate()
        {
            if (target)
            {
                UpdateCameraPosition();
            }
        }

        private void Update()
        {
            if (!target)
            {
                return;
            }
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseDeltaPos = mousePosition - mousePositionPrev;
            mousePositionPrev = mousePosition;

            if (Input.GetMouseButton(1))
            {
                Vector3 translation = -translationSpeed * camera.ScreenToWorldTranslation(mouseDeltaPos, Mathf.Max(10f, distance));
                translation = transform.TransformVector(translation);
                target.position += translation;
            }
            else if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    if (targetPlane && targetPlane.gameObject.activeSelf
                        && Maths.GetRayIntersectionWithPlane(transform.position, transform.forward,
                        Vector3.up, targetPlane.transform.position, out Vector3 isecPoint, out float rayDistance)
                        && rayDistance < 10f * distance)
                    {
                        target.position = isecPoint;
                        distance = rayDistance;
                    }
                    var eulerAngles = transform.eulerAngles;
                    eulerAngles.x -= mouseY * rotationSpeed;
                    eulerAngles.y += mouseX * rotationSpeed;
                    eulerAngles = Maths.AnglesModulo360(eulerAngles);
                    eulerAngles.x = Mathf.Clamp(eulerAngles.x, -90f, 90f);

                    transform.eulerAngles = eulerAngles;
                }
            }
            Vector2 mouseScrollDelta = Input.mouseScrollDelta;
            if (mouseScrollDelta.y != 0f)
            {
                if (camera.orthographic)
                {
                    camera.orthographicSize -= zoomSpeed * mouseScrollDelta.y;
                }
                else
                {
                    // calculate dist for half fov = 60/2 deg
                    float tanHalf60 = Mathf.Tan(Mathf.Deg2Rad * 30f);
                    float tanHalfFov = camera.GetTanHalfFov();
                    float r = distance * tanHalfFov / tanHalf60;
                    // update distance with zoom
                    r -= zoomSpeed * mouseScrollDelta.y * (1f + 0.04f * Mathf.Abs(r));
                    // calculate updated distance for current fov
                    distance = r * tanHalf60 / tanHalfFov;
                    distance = Mathf.Max(distance, 2f * camera.nearClipPlane + Mathv.Epsilon);
                }
            }
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            transform.position = target.position;
            transform.Translate(0f, 0f, -distance, Space.Self);
        }
    }
}
