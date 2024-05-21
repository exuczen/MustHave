using UnityEngine;
using UnityEngine.UI;

namespace MustHave.UI
{
    public class DebugPanel : UIScript
    {
        [SerializeField] private Text fpsText = default;
        [SerializeField] private Text debugText = default;

        private float fps = default;

        public string DebugText { set { debugText.text = value; } }

        public void SetDebugText(string text)
        {
            debugText.text = text;
        }

        private void Update()
        {
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            if (unscaledDeltaTime > 0f)
            {
                fps = Mathf.Lerp(fps, 1f / unscaledDeltaTime, 2f * unscaledDeltaTime);
                fpsText.text = "FPS: " + fps.ToString("F2");
            }
        }
    }
}
