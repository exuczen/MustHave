using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public class AppTargetFramerateEditor : EditorWindow
    {
        [MenuItem("Tools/AppTargetFramerateEditor")]
        private static void Init()
        {
            var window = GetWindow<AppTargetFramerateEditor>("TargetFramerateEditor");
            window.Show();
        }

        private void OnGUI()
        {
            Application.targetFrameRate = EditorGUILayout.IntSlider("target FPS", Application.targetFrameRate, 0, 1000);
            QualitySettings.vSyncCount = EditorGUILayout.IntSlider("vSyncCount", QualitySettings.vSyncCount, 0, 4);
        }
    }
}
