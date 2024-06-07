using UnityEngine;
using UnityEngine.Rendering;

namespace MustHave
{
    [CreateAssetMenu(fileName = "OutlineShaderSettings", menuName = "MustHave/Outline/OutlineShaderSettings")]
    public class OutlineShaderSettings : ScriptableObject
    {
        public enum DebugMode
        {
            DEBUG_NONE,
            DEBUG_SHAPES,
            DEBUG_CIRCLES,
            DEBUG_DEPTH
        }

        public ComputeShader Shader => shader;
        public bool DebugEnabled { get => debugEnabled; set => debugEnabled = value; }
        public DebugMode ShaderDebugMode => debugMode;

        [SerializeField]
        private ComputeShader shader = null;
        [SerializeField]
        private bool debugEnabled = false;
        [SerializeField]
        private bool debugEnabledOnInit = false;
        [SerializeField]
        private DebugMode debugMode = default;
        [SerializeField]
        private DebugMode debugModeOnInit = default;

        public void SetDebugModeOnInit()
        {
            debugEnabledOnInit = debugEnabled;

            SetDebugMode(debugModeOnInit = debugMode);
        }

        public void SetDebugModeFromInit()
        {
            debugEnabled = debugEnabledOnInit;

            SetDebugMode(debugModeOnInit);
        }

        public void SetDebugMode(DebugMode debugMode)
        {
            if (!shader)
            {
                return;
            }
            if (!debugEnabled)
            {
                debugMode = DebugMode.DEBUG_NONE;
            }
            //var prevKeyword = new LocalKeyword(shader, this.debugMode.ToString());
            var keyword = new LocalKeyword(shader, debugMode.ToString());

            /* Disabled keyword validity check on purpose - there are scenarios, where skipping 
             * invalid keyword causes missing kernel errors after shader recompilation.
             * Using string keyword names instead of LocalKeyword struct for the same reason.  */

            //if (!keyword.isValid || !prevKeyword.isValid)
            //{
            //    if (!keyword.isValid)
            //    {
            //        Debug.LogError($"{GetType().Name}.SetDebugMode: Invalid keyword: {keyword}");
            //    }
            //    if (!prevKeyword.isValid)
            //    {
            //        Debug.LogError($"{GetType().Name}.SetDebugMode: Invalid keyword: {prevKeyword}");
            //    }
            //    return;
            //}
            if (this.debugMode == debugMode && shader.IsKeywordEnabled(keyword))
            {
                return;
            }
            shader.DisableKeyword(this.debugMode.ToString());
            this.debugMode = debugMode;
            debugModeOnInit = Application.isPlaying ? debugModeOnInit : debugMode;
            shader.EnableKeyword(debugMode.ToString());
        }
    }
}