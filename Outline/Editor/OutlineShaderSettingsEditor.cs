using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(OutlineShaderSettings))]
    public class OutlineShaderSettingsEditor : Editor
    {
        private const string UndoNotRecordedTooltipMessage = "Debug Shader Mode changes are not Undo recorded due to ComputeShader keyword errors on Undo.";

        private SerializedProperty shaderProperty;

        private void OnEnable()
        {
            shaderProperty = serializedObject.FindProperty("shader");
        }

        public void OnInspectorGUI(bool showShader, out bool modified)
        {
            var settings = target as OutlineShaderSettings;

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
            bool changed = EditorGUI.EndChangeCheck();
            modified = serializedObject.ApplyModifiedProperties() || changed;

            if (modified)
            {
                if (Application.isPlaying)
                {
                    settings.SetDebugMode(debugMode);
                }
                else
                {
                    settings.SetDebugModeOnInit(debugMode);

                    EditorUtils.SetSceneOrObjectDirty(target);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUI(true, out _);
        }
    }
}
