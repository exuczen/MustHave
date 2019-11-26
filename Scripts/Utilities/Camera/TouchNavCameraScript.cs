using MustHave.Utilities;
using UnityEngine;

namespace MustHave.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class TouchNavCameraScript : MonoBehaviour
    {
        [SerializeField] private float _translationSpeed = default;

        private Camera _camera = default;

        private void Awake()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            _camera = GetComponent<Camera>();
#else
            Destroy(this);
#endif
        }

        private void Update()
        {
            if (Input.touchCount >= 2)
            {
                Touch[] touches = Input.touches;
                Vector2 deltaPositionMax = Vector2.zero;
                foreach (var touch in touches)
                {
                    deltaPositionMax.x = Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(deltaPositionMax.x) ? touch.deltaPosition.x : deltaPositionMax.x;
                    deltaPositionMax.y = Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(deltaPositionMax.y) ? touch.deltaPosition.y : deltaPositionMax.y;
                }
                Vector3 translation = -_translationSpeed * _camera.ScreenToWorldTranslation(deltaPositionMax);
                transform.Translate(translation, Space.Self);
            }
        }
    }
}
