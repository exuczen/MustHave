using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(OutlineCamera))]
    public class OutlineCameraEditor : Editor
    {
        private OutlineShaderSettingsEditor shaderSettingsEditor = null;

        private void OnEnable()
        {
            var outlineCamera = target as OutlineCamera;
            if (outlineCamera)
            {
                Editor editor = shaderSettingsEditor;
                CreateCachedEditor(outlineCamera.ShaderSettings, typeof(OutlineShaderSettingsEditor), ref editor);
                shaderSettingsEditor = editor as OutlineShaderSettingsEditor;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (shaderSettingsEditor)
            {
                var outlineCamera = target as OutlineCamera;

                outlineCamera.ShaderSettingsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(outlineCamera.ShaderSettingsExpanded, "Shader Settings");

                if (outlineCamera.ShaderSettingsExpanded)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.ObjectField("Shader Settings", outlineCamera.ShaderSettings, typeof(OutlineShaderSettings), true);
                    shaderSettingsEditor.OnInspectorGUI(false, out _);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
