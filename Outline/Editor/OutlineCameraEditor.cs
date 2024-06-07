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
                shaderSettingsEditor.OnInspectorGUI(false, out _);
            }
        }
    }
}
