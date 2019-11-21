﻿using UnityEngine;
using UnityEngine.UI;

namespace MustHave.UI
{
    public class DebugPanel : UIScript
    {
        [SerializeField] private Text fpsText = default;
        [SerializeField] private Text debugText = default;

        private float fpsCounter;
        private float fpsAvgStartTime;

        public string DebugText { set { debugText.text = value; } }

        public void SetDebugText(string text)
        {
            debugText.text = text;
        }

        protected override void Start()
        {
            fpsCounter = 0;
            fpsAvgStartTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime > 0.0001f ? Time.unscaledDeltaTime : 1f;
            float fps = 1f / deltaTime;
            deltaTime = Time.realtimeSinceStartup - fpsAvgStartTime;
            deltaTime = deltaTime > 0.0001f ? deltaTime : 1f;
            fpsCounter++;
            float fpsAvg = fpsCounter / deltaTime;
            if (deltaTime >= 3f)
            {
                fpsAvgStartTime = Time.realtimeSinceStartup;
                fpsCounter = 0;
            }
            fpsText.text = string.Concat("fps=", fps.ToString("n2"), "\n", "fpsAvg=", fpsAvg.ToString("n2"));
        }
    }
}
