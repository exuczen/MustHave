#if UNITY_PIPELINE_URP
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MustHave
{
    [Serializable]
    public class RenderPassSettings
    {
        public RenderPassEvent RenderPassEvent { get => renderPassEvent; set => renderPassEvent = value; }

        [SerializeField]
        private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }
}
#endif