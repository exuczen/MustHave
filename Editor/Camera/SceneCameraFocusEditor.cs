using MustHave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(SceneCameraFocus))]
    public class SceneCameraFocusEditor : Editor
    {
        private void OnSceneGUI()
        {
            Event currEvent = Event.current;
            if (currEvent.type == EventType.KeyDown && currEvent.keyCode == KeyCode.F)
            {
                SceneCameraFocus cameraFocus = target as SceneCameraFocus;
                if (cameraFocus.transform is RectTransform)
                {
                    cameraFocus.AlignToForwardAxis();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneCameraFocus cameraFocus = target as SceneCameraFocus;
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Forward Axis Sign: ");
            int selectedFwdAxisSignIndex = (cameraFocus.ForwardAxisSign + 1) >> 1;
            selectedFwdAxisSignIndex = GUILayout.SelectionGrid(selectedFwdAxisSignIndex, new string[] { " - ", " + " }, 2);
            cameraFocus.ForwardAxisSign = (selectedFwdAxisSignIndex << 1) - 1;
            GUILayout.EndHorizontal();
        }
    }
}
