﻿using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(ComputeOutlineShaderSettings))]
    public class ComputeOutlineShaderSettingsEditor : Editor
    {
        private const string UndoNotRecordedTooltipMessage = "Shader settings changes are not Undo recorded due to ComputeShader keyword errors on Undo.";

        private SerializedProperty computeShaderProperty;
        private SerializedProperty circleSpriteMaterialProperty;
        private SerializedProperty smoothRadiusProperty;
        private SerializedProperty smoothPowerProperty;
        private SerializedProperty smoothWeightsPowerProperty;

        private void OnEnable()
        {
            computeShaderProperty = serializedObject.FindProperty("computeShader");
            circleSpriteMaterialProperty = serializedObject.FindProperty("circleSpriteMaterial");
            smoothRadiusProperty = serializedObject.FindProperty("smoothRadius");
            smoothPowerProperty = serializedObject.FindProperty("smoothPower");
            smoothWeightsPowerProperty = serializedObject.FindProperty("smoothWeightsPower");
        }

        public void OnInspectorGUI(bool showShader)
        {
            var settings = target as ComputeOutlineShaderSettings;

            bool setDirty = false;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (showShader)
            {
                EditorGUILayout.PropertyField(computeShaderProperty);
            }
            settings.DebugEnabled = EditorGUILayout.Toggle(new GUIContent("Debug Shader", UndoNotRecordedTooltipMessage), settings.DebugEnabled);

            var debugMode = settings.ShaderDebugMode;

            if (settings.DebugEnabled)
            {
                debugMode = (ComputeOutlineShaderSettings.DebugMode)EditorGUILayout.EnumPopup(new GUIContent("Debug Mode", UndoNotRecordedTooltipMessage), debugMode);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    settings.SetDebugMode(debugMode);
                }
                else
                {
                    settings.SetDebugModeOnInit(debugMode);

                    setDirty = true;
                }
            }
            EditorGUILayout.PropertyField(circleSpriteMaterialProperty);

            EditorGUI.BeginChangeCheck();

            var circleShaderVariant = settings.CirclesShaderVariant;

            circleShaderVariant = (ComputeOutlineShaderSettings.CircleShaderVariant)EditorGUILayout.EnumPopup(new GUIContent("Circle Shader Variant", UndoNotRecordedTooltipMessage), circleShaderVariant);

            if (EditorGUI.EndChangeCheck())
            {
                settings.SetCircleShaderVariant(circleShaderVariant);

                setDirty |= !Application.isPlaying;
            }
            bool modified = serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            EditorGUILayout.PropertyField(smoothRadiusProperty);
            EditorGUILayout.PropertyField(smoothPowerProperty);
            EditorGUILayout.PropertyField(smoothWeightsPowerProperty);

            bool smoothModified = serializedObject.ApplyModifiedProperties();
            if (smoothModified)
            {
                var outlineCamera = IComputeOutlineCamera.Instance;
                if (outlineCamera)
                {
                    outlineCamera.SetSmoothWeights(/*true*/);
                }
            }
            modified |= smoothModified;
            setDirty |= modified;

            if (setDirty)
            {
                EditorUtility.SetDirty(settings);
            }
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUI(true);
        }
    }
}
