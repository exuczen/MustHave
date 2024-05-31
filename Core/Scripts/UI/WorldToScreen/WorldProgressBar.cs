using UnityEngine;
using UnityEngine.UI;

namespace MustHave.UI
{
    public class WorldProgressBar : ProgressBar, IPoolable
    {
        public UIWorldToScreen WorldToScreen => worldToScreen;

        [SerializeField]
        private UIWorldToScreen worldToScreen = null;

        public void Setup(Transform worldTransform, Transform parent)
        {
            if (worldTransform)
            {
                worldToScreen.WorldTransform = worldTransform;
            }
            transform.SetParent(parent);
        }

        public void OnReturnToPool()
        {
            WorldToScreen.WorldTransform = null;
            FillImageColor = Color.white;
            Slider.value = 0f;
        }
    }
}
