using UnityEngine;

namespace MustHave
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupLayoutRebuilderHACK : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup = null;
        [SerializeField]
        private int delayFramesCount = 3;

        private void OnEnable()
        {
            canvasGroup.ForceRebuildLayoutImmediate(this, delayFramesCount);
        }
    } 
}
