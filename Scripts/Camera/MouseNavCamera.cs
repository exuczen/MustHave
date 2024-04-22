using UnityEngine;

namespace MustHave
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class MouseNavCamera : MonoBehaviour
    {
        private const float ScrollScale = 0.04f;

        [SerializeField] private bool keyboardMovement = true;
        [SerializeField] private float keyTranslationSpeed = 10f;
        [SerializeField] private float panTranslationSpeed = 10f;
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
            SetOrthoCameraSize(camera.orthographicSize, true);
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
            float tanHalfFov = camera.GetTanHalfFov();
            float tanHalf60 = Mathf.Tan(Mathf.Deg2Rad * 30f);

            if (Application.isPlaying)
            {
                var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                bool rotatingAroundTarget = Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt);
                bool rotatingAroundSelf = Input.GetMouseButton(1);
                bool panning = Input.GetMouseButton(2);

                if (panning)
                {
                    Vector3 translation = -panTranslationSpeed * camera.ScreenToWorldTranslation(mouseDelta, Mathf.Max(1f, distance));
                    translation = transform.TransformVector(translation);
                    target.position += translation;
                }
                else if (rotatingAroundTarget || rotatingAroundSelf)
                {
                    if (!rotatingAroundSelf &&
                        targetPlane && targetPlane.gameObject.activeSelf &&
                        Maths.GetRayIntersectionWithPlane(transform.position, transform.forward,
                        Vector3.up, targetPlane.transform.position, out Vector3 isecPoint, out float rayDistance) &&
                        rayDistance < 10f * distance)
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

                    if (rotatingAroundSelf)
                    {
                        target.position = transform.position + transform.forward * distance;
                    }
                }
                Vector2 mouseScrollDelta = Input.mouseScrollDelta;
                if (mouseScrollDelta.y != 0f)
                {
                    float scrollDelta = ScrollScale * zoomSpeed * mouseScrollDelta.y;

                    if (camera.orthographic)
                    {
                        SetOrthoCameraSize(camera.orthographicSize * (1f - scrollDelta), true);
                    }
                    else
                    {
                        // calculate dist for half fov = 60/2 deg
                        float r = distance * tanHalfFov / tanHalf60;
                        // update distance with zoom
                        r -= scrollDelta * Mathf.Abs(r);
                        // calculate updated distance for current fov
                        distance = r * tanHalf60 / tanHalfFov;
                        distance = Mathf.Max(distance, 2f * camera.nearClipPlane + Mathv.Epsilon);
                    }
                }
                if (keyboardMovement)
                {
                    var translation = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
                    translation.y -= Input.GetKey(KeyCode.Q) ? 1f : 0f;
                    translation.y += Input.GetKey(KeyCode.E) ? 1f : 0f;
                    translation *= Time.deltaTime * keyTranslationSpeed;
                    translation *= Input.GetKey(KeyCode.LeftShift) ? 3f : 1f;

                    if (Mathf.Abs(translation.z) > Mathf.Epsilon)
                    {
                        if (camera.orthographic)
                        {
                            SetOrthoCameraSize(camera.orthographicSize - translation.z);
                        }
                        else
                        {
                            translation.z *= tanHalf60 / tanHalfFov;
                        }
                    }
                    //Debug.Log($"{GetType().Name}.{translation}");
                    translation = transform.TransformVector(translation);
                    target.position += translation;
                }
            }
            if (cameraFov != camera.fieldOfView)
            {
                float tanHalfFovPrev = Mathf.Tan(Mathf.Deg2Rad * cameraFov * 0.5f);
                // tanHalfFovPrev * distance = tanHalfFov * newDistance
                distance *= tanHalfFovPrev / tanHalfFov;
                cameraFov = camera.fieldOfView;
            }
            if (camera.orthographic != cameraOrthographic)
            {
                cameraOrthographic = camera.orthographic;

                if (cameraOrthographic)
                {
                    SetOrthoCameraSize(distance * tanHalfFov, true);
                }
                else
                {
                    distance = camera.orthographicSize / tanHalfFov;
                }
            }
            UpdateCameraPosition();
        }

        private void SetOrthoCameraSize(float orthoSize, bool setDistance = false)
        {
            if (camera.orthographic)
            {
                orthoSize = Mathf.Max(Mathv.Epsilon, orthoSize);
                camera.orthographicSize = orthoSize;

                if (setDistance)
                {
                    distance = 10f * orthoSize;
                }
            }
        }

        private void UpdateCameraPosition()
        {
            transform.position = target.position;
            transform.Translate(0f, 0f, -distance, Space.Self);
        }
    }
}
