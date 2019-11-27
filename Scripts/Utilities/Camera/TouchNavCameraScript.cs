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
            int touchCount = Input.touchCount;
            if (touchCount >= 2)
            {
                Touch[] touches = Input.touches;
                Vector2 deltaPosition = Vector2.zero;
                float[] deltaSqrLengths = new float[touchCount];
                float deltaSqrLengthSum = 0f;
                for (int i = 0; i < touchCount; i++)
                {
                    deltaSqrLengths[i] = touches[i].deltaPosition.sqrMagnitude;
                    deltaSqrLengthSum += deltaSqrLengths[i];
                }
                if (deltaSqrLengthSum > 0f)
                {
                    for (int i = 0; i < touchCount; i++)
                    {
                        deltaPosition += deltaSqrLengths[i] * touches[i].deltaPosition;
                    }
                    deltaPosition /= deltaSqrLengthSum;
                    Vector3 translation = -_translationSpeed * _camera.ScreenToWorldTranslation(deltaPosition);
                    transform.Translate(translation, Space.Self);
                }
            }
        }
    }
}
