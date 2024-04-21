using UnityEngine;

namespace MustHave
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class MouseNavCamera : MonoBehaviour
    {
        private const float ScrollScale = 0.04f;

        [SerializeField] private float translationSpeed = 10f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float rotationSpeed = 4f;
        [SerializeField] private float distance = 10f;
        [SerializeField] private Transform target = null;
        [SerializeField] private Transform targetPlane = null;
        [SerializeField, HideInInspector] private new Camera camera = default;

        private bool cameraOrthographic = default;
        private float cameraFov = 0f;

        private void OnEnable()
        {
            camera = GetComponent<Camera>();
            cameraOrthographic = camera.orthographic;
            cameraFov = camera.fieldOfView;

            if (target)
            {
                UpdateCameraPosition();
            }
            SetOrthoCameraDistance();
        }

        private void OnValidate()
        {
            OnEnable();
        }

        private void Update()
        {
            if (!target)
            {
                return;
            }
            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButton(1))
            {
                Vector3 translation = -translationSpeed * camera.ScreenToWorldTranslation(mouseDelta, Mathf.Max(1f, distance));
                translation = transform.TransformVector(translation);
                target.position += translation;
            }
            else if (Input.GetMouseButton(0))
            {
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
                    eulerAngles.x -= mouseDelta.y * rotationSpeed;
                    eulerAngles.y += mouseDelta.x * rotationSpeed;
                    eulerAngles = Maths.AnglesModulo360(eulerAngles);
                    eulerAngles.x = Mathf.Clamp(eulerAngles.x, -90f, 90f);

                    transform.eulerAngles = eulerAngles;
                }
            }
            Vector2 mouseScrollDelta = Input.mouseScrollDelta;
            if (mouseScrollDelta.y != 0f)
            {
                float scrollDelta = ScrollScale * zoomSpeed * mouseScrollDelta.y;

                if (camera.orthographic)
                {
                    float orthoSize = camera.orthographicSize;
                    orthoSize -= scrollDelta * orthoSize;
                    orthoSize = Mathf.Max(Mathv.Epsilon, orthoSize);
                    camera.orthographicSize = orthoSize;
                    SetOrthoCameraDistance();
                }
                else
                {
                    // calculate dist for half fov = 60/2 deg
                    float tanHalf60 = Mathf.Tan(Mathf.Deg2Rad * 30f);
                    float tanHalfFov = camera.GetTanHalfFov();
                    float r = distance * tanHalfFov / tanHalf60;
                    // update distance with zoom
                    r -= scrollDelta * Mathf.Abs(r);
                    // calculate updated distance for current fov
                    distance = r * tanHalf60 / tanHalfFov;
                    distance = Mathf.Max(distance, 2f * camera.nearClipPlane + Mathv.Epsilon);
                }
            }

            if (cameraFov != camera.fieldOfView)
            {
                float tanHalfFovPrev = Mathf.Tan(Mathf.Deg2Rad * cameraFov * 0.5f);
                float tanHalfFov = camera.GetTanHalfFov();
                // tanHalfFovPrev * distance = tanHalfFov * newDistance
                distance *= tanHalfFovPrev / tanHalfFov;
                cameraFov = camera.fieldOfView;
            }
            if (camera.orthographic != cameraOrthographic)
            {
                cameraOrthographic = camera.orthographic;
                float tanHalfFov = camera.GetTanHalfFov();

                if (cameraOrthographic)
                {
                    camera.orthographicSize = distance * tanHalfFov;
                    SetOrthoCameraDistance();
                }
                else
                {
                    distance = camera.orthographicSize / tanHalfFov;
                }
            }
            UpdateCameraPosition();
        }

        private void SetOrthoCameraDistance()
        {
            if (camera.orthographic)
            {
                distance = 10f * camera.orthographicSize;
            }
        }


        private void UpdateCameraPosition()
        {
            transform.position = target.position;
            transform.Translate(0f, 0f, -distance, Space.Self);
        }
    }
}
