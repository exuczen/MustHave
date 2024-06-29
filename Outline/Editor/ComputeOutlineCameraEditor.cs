using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(ComputeOutlineCamera))]
    public class ComputeOutlineCameraEditor : Editor
    {
        private ComputeOutlineShaderSettingsEditor shaderSettingsEditor = null;

        private void OnEnable()
        {
            var outlineCamera = target as ComputeOutlineCamera;
            if (outlineCamera)
            {
                Editor editor = shaderSettingsEditor;
                CreateCachedEditor(outlineCamera.ShaderSettings, typeof(ComputeOutlineShaderSettingsEditor), ref editor);
                shaderSettingsEditor = editor as ComputeOutlineShaderSettingsEditor;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (shaderSettingsEditor)
            {
                var outlineCamera = target as ComputeOutlineCamera;

                outlineCamera.ShaderSettingsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(outlineCamera.ShaderSettingsExpanded, "Shader Settings");

                if (outlineCamera.ShaderSettingsExpanded)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.ObjectField("Shader Settings", outlineCamera.ShaderSettings, typeof(ComputeOutlineShaderSettings), true);
                    shaderSettingsEditor.OnInspectorGUI(false);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
