using UnityEngine;

namespace MustHave.UI
{
    public class UIWorldToScreen : MonoBehaviour
    {
        public Transform WorldTransform { get; set; }

        public void UpdatePosition(Camera camera)
        {
            Vector3 screenPoint = camera.WorldToScreenPointWithYOffset(WorldTransform.position, 1f, 0.06f);
            screenPoint.z = 0f;
            transform.position = screenPoint;
        }
    }
}
