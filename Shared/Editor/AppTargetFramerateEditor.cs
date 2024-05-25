using UnityEditor;
using UnityEngine;
#if MUSTHAVE_SHOW_TOOLS
using MenuItem = UnityEditor.MenuItem;
#else
using MenuItem = MustHave.InactiveMenuItem;
#endif

namespace MustHave
{
    public class AppTargetFramerateEditor : EditorWindow
    {
        [MenuItem("Tools/Target Framerate Editor")]
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
