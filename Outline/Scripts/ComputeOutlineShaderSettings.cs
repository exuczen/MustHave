﻿using UnityEngine;
using UnityEngine.Rendering;

namespace MustHave
{
    [CreateAssetMenu(fileName = "ComputeOutlineShaderSettings", menuName = "MustHave/Outline/ComputeOutlineShaderSettings")]
    public class ComputeOutlineShaderSettings : ScriptableObject
    {
        public enum DebugMode
        {
            DEBUG_NONE,
            DEBUG_SHAPES,
            DEBUG_CIRCLES,
            DEBUG_DEPTH
        }

        public enum CircleShaderVariant
        {
            INSTANCE_DATA_VARIANT,
            INSTANCE_MATRIX_VARIANT
        }

        public ComputeShader ComputeShader => computeShader;
        public bool DebugEnabled { get => debugEnabled; set => debugEnabled = value; }
        public DebugMode ShaderDebugMode => debugMode;
        public CircleShaderVariant CirclesShaderVariant => circleShaderVariant;
        public int SmoothRadius { get => smoothRadius; set => smoothRadius = value; }
        public int SmoothPower{ get => smoothPower; set => smoothPower = value; }
        public int SmoothWeightsPower { get => smoothWeightsPower; set => smoothWeightsPower = value; }

        [SerializeField]
        private ComputeShader computeShader = null;
        [SerializeField]
        private bool debugEnabled = false;
        [SerializeField]
        private bool debugEnabledOnInit = false;
        [SerializeField]
        private DebugMode debugMode = default;
        [SerializeField]
        private DebugMode debugModeOnInit = default;
        [SerializeField]
        private Material circleSpriteMaterial = null;
        [SerializeField]
        private CircleShaderVariant circleShaderVariant = CircleShaderVariant.INSTANCE_MATRIX_VARIANT;
        [SerializeField, Range(0, 3)]
        private int smoothRadius = 3;
        [SerializeField, Range(1, 6)]
        private int smoothPower = 4;
        [SerializeField, Range(1, 4)]
        private int smoothWeightsPower = 4;

        public void SetDebugModeOnInit()
        {
            SetDebugModeOnInit(debugMode);
        }

        public void SetDebugModeOnInit(DebugMode debugMode)
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
            if (!computeShader)
            {
                return;
            }
            if (!debugEnabled)
            {
                debugMode = DebugMode.DEBUG_NONE;
            }
            //var prevKeyword = new LocalKeyword(shader, this.debugMode.ToString());
            var keyword = new LocalKeyword(computeShader, debugMode.ToString());

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
            if (this.debugMode == debugMode && computeShader.IsKeywordEnabled(keyword))
            {
                return;
            }
            computeShader.DisableKeyword(this.debugMode.ToString());
            this.debugMode = debugMode;
            debugModeOnInit = Application.isPlaying ? debugModeOnInit : debugMode;
            computeShader.EnableKeyword(debugMode.ToString());
        }

        public void SetCircleShaderVariant()
        {
            SetCircleShaderVariant(circleShaderVariant);
        }

        public void SetCircleShaderVariant(CircleShaderVariant variant)
        {
            if (circleShaderVariant == variant && circleSpriteMaterial.IsKeywordEnabled(variant.ToString()))
            {
                return;
            }
            circleSpriteMaterial.DisableKeyword(circleShaderVariant.ToString());
            circleShaderVariant = variant;
            circleSpriteMaterial.EnableKeyword(circleShaderVariant.ToString());
        }
    }
}