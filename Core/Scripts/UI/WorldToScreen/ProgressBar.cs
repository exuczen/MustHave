using UnityEngine;
using UnityEngine.UI;

namespace MustHave.UI
{
    [RequireComponent(typeof(Slider))]
    public class ProgressBar : MonoBehaviour
    {
        public Slider Slider => slider;
        public Color FillImageColor { set => fillImage.color = value; }

        [SerializeField]
        private Slider slider = null;
        [SerializeField]
        private Image fillImage = null;

    }
}
