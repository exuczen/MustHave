using MustHave.Utilities;
using UnityEngine;

namespace MustHave
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
            int touchCount = Input.touchCount;
            if (touchCount >= 2)
            {
                Touch[] touches = Input.touches;
                Vector2 deltaPosition = Vector2.zero;
                Vector2 absDeltaSum = Vector2.zero;
                for (int i = 0; i < touchCount; i++)
                {
                    float deltaPosX = touches[i].deltaPosition.x;
                    float deltaPosY = touches[i].deltaPosition.y;
                    float absDeltaPosX = Mathf.Abs(deltaPosX);
                    float absDeltaPosY = Mathf.Abs(deltaPosY);
                    deltaPosition.x += absDeltaPosX * deltaPosX;
                    deltaPosition.y += absDeltaPosY * deltaPosY;
                    absDeltaSum.x += absDeltaPosX;
                    absDeltaSum.y += absDeltaPosY;
                }
                deltaPosition.x = absDeltaSum.x > 0f ? deltaPosition.x / absDeltaSum.x : 0f;
                deltaPosition.y = absDeltaSum.y > 0f ? deltaPosition.y / absDeltaSum.y : 0f;
                if (deltaPosition != Vector2.zero)
                {
                    Vector3 translation = -_translationSpeed * _camera.ScreenToWorldTranslation(deltaPosition);
                    transform.Translate(translation, Space.Self);
                }
            }
        }
    }
}
