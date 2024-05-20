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
        [SerializeField, Range(0.01f, 2f)] private float keyTranslationMlp = 1f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float rotationSpeed = 4f;
        [SerializeField] private float distance = 10f;
        [SerializeField] private Transform target = null;
        [SerializeField] private Transform targetPlane = null;
#if UNITY_EDITOR
        [SerializeField, HideInInspector] private new Camera camera = default;
#else
        [SerializeField, HideInInspector] private Camera camera = default;
#endif
        private readonly GUIStyle guiStyle = new();

        private bool cameraOrthoPrev = default;
        private float cameraFovPrev = 0f;

        private Vector3 mousePositionPrev = default;

        private bool appGainedFocus = false;

        private float keyTranslationMlpChangeTime = -1f;

        private void OnEnable()
        {
            camera = GetComponent<Camera>();

            SetPreviousData();

            if (target)
            {
                UpdateCameraPosition();
            }
            SetOrthoCameraSize(camera.orthographicSize, true);

            keyTranslationMlpChangeTime = -1f;
        }

        private void OnValidate()
        {
            if (enabled)
            {
                OnEnable();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            SetPreviousData();
            appGainedFocus = true;
        }

        private void OnGUI()
        {
            static Rect getMiddleRect(int width = 60, int height = 20) => new((Screen.width - width) >> 1, (Screen.height - height) >> 1, width, height);

            if (keyTranslationMlpChangeTime > 0f)
            {
                if (Time.time - keyTranslationMlpChangeTime <= 1f)
                {
                    guiStyle.alignment = TextAnchor.MiddleCenter;
                    guiStyle.normal.textColor = Color.white;
                    guiStyle.fontSize = 30;
                    GUI.Box(getMiddleRect(200, 100), string.Empty);
                    GUI.Box(getMiddleRect(100, 100), $" x {keyTranslationMlp:f2}", guiStyle);
                }
                else
                {
                    keyTranslationMlpChangeTime = -1f;
                }
            }
        }

        private void Update()
        {
            if (!target)
            {
                return;
            }
            float tanHalfFov = camera.GetTanHalfFov();
            float tanHalf60 = Mathf.Tan(Mathf.Deg2Rad * 30f);

            var mousePosition = Input.mousePosition;

            if (Application.isPlaying)
            {
                bool rotatingAroundTarget = Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt);
                bool rotatingAroundSelf = Input.GetMouseButton(1);
                bool panning = Input.GetMouseButton(2) && !appGainedFocus;

                if (panning)
                {
                    var mouseDelta = mousePosition - mousePositionPrev;

                    Vector3 translation = -camera.ScreenToWorldTranslation(mouseDelta, Mathf.Max(1f, distance));
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
                    var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

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
                    float scrollDelta = ScrollScale * mouseScrollDelta.y;

                    if (!Input.GetMouseButton(1))
                    {
                        scrollDelta *= zoomSpeed;

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
                    else if (keyboardMovement)
                    {
                        keyTranslationMlp += scrollDelta;
                        keyTranslationMlp = (int)(keyTranslationMlp / ScrollScale + 0.5f) * ScrollScale;
                        keyTranslationMlp = Mathf.Clamp(keyTranslationMlp, 0.01f, 2f);
                        keyTranslationMlpChangeTime = Time.time;
                    }
                }
                if (keyboardMovement)
                {
                    var translation = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
                    translation.y -= Input.GetKey(KeyCode.Q) ? 1f : 0f;
                    translation.y += Input.GetKey(KeyCode.E) ? 1f : 0f;
                    translation *= Time.deltaTime * keyTranslationSpeed * keyTranslationMlp;
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
            if (cameraFovPrev != camera.fieldOfView)
            {
                float tanHalfFovPrev = Mathf.Tan(Mathf.Deg2Rad * cameraFovPrev * 0.5f);
                // tanHalfFovPrev * distance = tanHalfFov * newDistance
                distance *= tanHalfFovPrev / tanHalfFov;
            }
            if (cameraOrthoPrev != camera.orthographic)
            {
                if (camera.orthographic)
                {
                    SetOrthoCameraSize(distance * tanHalfFov, true);
                }
                else
                {
                    distance = camera.orthographicSize / tanHalfFov;
                }
            }
            UpdateCameraPosition();
            SetPreviousData();

            appGainedFocus = false;
        }

        private void SetPreviousData()
        {
            cameraOrthoPrev = camera.orthographic;
            cameraFovPrev = camera.fieldOfView;
            mousePositionPrev = Input.mousePosition;
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

