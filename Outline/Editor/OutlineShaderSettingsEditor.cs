using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(OutlineShaderSettings))]
    public class OutlineShaderSettingsEditor : Editor
    {
        private const string UndoNotRecordedTooltipMessage = "Shader settings changes are not Undo recorded due to ComputeShader keyword errors on Undo.";

        private SerializedProperty shaderProperty;
        private SerializedProperty circleSpriteMaterialProperty;

        private void OnEnable()
        {
            shaderProperty = serializedObject.FindProperty("shader");
            circleSpriteMaterialProperty = serializedObject.FindProperty("circleSpriteMaterial");
        }

        public void OnInspectorGUI(bool showShader)
        {
            var settings = target as OutlineShaderSettings;

            bool setDirty = false;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (showShader)
            {
                EditorGUILayout.PropertyField(shaderProperty);
            }
            settings.DebugEnabled = EditorGUILayout.Toggle(new GUIContent("Debug Shader", UndoNotRecordedTooltipMessage), settings.DebugEnabled);

            var debugMode = settings.ShaderDebugMode;

            if (settings.DebugEnabled)
            {
                debugMode = (OutlineShaderSettings.DebugMode)EditorGUILayout.EnumPopup(new GUIContent("Debug Mode", UndoNotRecordedTooltipMessage), debugMode);
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

            circleShaderVariant = (OutlineShaderSettings.CircleShaderVariant)EditorGUILayout.EnumPopup(new GUIContent("Circle Shader Variant", UndoNotRecordedTooltipMessage), circleShaderVariant);

            if (EditorGUI.EndChangeCheck())
            {
                settings.SetCircleShaderVariant(circleShaderVariant);

                setDirty |= !Application.isPlaying;
            }
            bool modified = serializedObject.ApplyModifiedProperties();
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
