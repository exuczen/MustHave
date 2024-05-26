using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public class MustHaveExporterWindow : EditorWindow
    {
        [SerializeField]
        private DefineSymbols editorDefineSymbols = null;
        [SerializeField]
        private DefineSymbols buildDefineSymbols = null;
        [SerializeField]
        private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;

        private Editor editorDefineSymbolsEditor = null;
        private Editor buildDefineSymbolsEditor = null;

        private Vector2 scrollViewPosition = default;

        [MenuItem("Tools/MustHave/MustHave Exporter")]
        private static void ShowWindow()
        {
            GetWindow<MustHaveExporterWindow>("MustHave Exporter");
        }

        private void OnGUI()
        {

            int step = 1;
            var labelStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = Color.white;

            scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition, false, true);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"{step++}. Set Scripting Define Symbols for build", labelStyle);
            {
                DefineSymbolsEditorWindow.OnDefineSymbolsGUI("Build", buildDefineSymbols, ref buildDefineSymbolsEditor);
            }
            if (GUILayout.Button($"{step++}. Build {buildTarget}"))
            {
                BuildUtils.BuildActiveScene(buildTarget);
            }
            // Build Target
            {
                buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildTarget);
            }
            if (GUILayout.Button($"{step++}. Enable MustHave DLLs platforms for export"))
            {
                MustHaveExporter.EnableMustHaveLibsPlatforms();
            }
            if (GUILayout.Button($"{step++}. Export MustHave Outline Package"))
            {
                MustHaveExporter.ExportMustHaveOutlinePackage();
            }
            if (GUILayout.Button($"{step++}. Disable MustHave DLLs platforms"))
            {
                MustHaveExporter.DisableMustHaveLibsPlatforms();
            }
            EditorGUILayout.LabelField($"{step++}. Restore Scripting Define Symbols in editor", labelStyle);
            {
                DefineSymbolsEditorWindow.OnDefineSymbolsGUI("Editor", editorDefineSymbols, ref editorDefineSymbolsEditor);
            }
            GUILayout.EndScrollView();
        }
    }
}
