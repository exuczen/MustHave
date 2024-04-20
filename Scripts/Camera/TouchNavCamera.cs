using UnityEngine;

namespace MustHave
{
    [RequireComponent(typeof(Camera))]
    public class TouchNavCamera : MonoBehaviour
    {
        [SerializeField] private float translationSpeed = default;

        private new Camera camera = default;

        private void Awake()
        {
            camera = GetComponent<Camera>();
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
                    Vector3 translation = -translationSpeed * camera.ScreenToWorldTranslation(deltaPosition);
                    transform.Translate(translation, Space.Self);
                }
            }
        }
    }
}
