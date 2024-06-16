using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(OutlineObject))]
    public class OutlineObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var outlineCamera = IOutlineCameraSingleton.Instance;
            if (outlineCamera)
            {
                EditorGUI.BeginChangeCheck();

                int lineThickness = EditorGUILayout.IntSlider("Line Thickness", outlineCamera.LineThickness, 1, OutlineCamera.LineMaxThickness);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(outlineCamera, outlineCamera.name);

                    outlineCamera.LineThickness = lineThickness;

                    var serializedCamera = new SerializedObject(outlineCamera);
                    serializedCamera.ApplyModifiedProperties();

                    if (!EditorApplication.isPlaying)
                    {
                        EditorUtils.SetSceneOrObjectDirty(outlineCamera);
                        EditorApplication.QueuePlayerLoopUpdate();
                    }
                }
            }
        }
    }
}
