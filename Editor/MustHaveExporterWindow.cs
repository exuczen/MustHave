using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public class MustHaveExporterWindow : EditorWindow
    {
        [SerializeField]
        private DefineSymbols defineSymbols = null;

        private Editor defineSymbolsEditor = null;

        [MenuItem("Tools/MustHave/MustHave Exporter")]
        private static void ShowWindow()
        {
            GetWindow<MustHaveExporterWindow>("MustHave Exporter");
        }

        private void OnGUI()
        {
            var defineSymbolsType = typeof(DefineSymbols);
            defineSymbols = EditorGUILayout.ObjectField(defineSymbolsType.Name, defineSymbols, defineSymbolsType, true) as DefineSymbols;
            if (!defineSymbols)
            {
                return;
            }
            if (!defineSymbolsEditor)
            {
                defineSymbolsEditor = Editor.CreateEditor(defineSymbols);
            }
            int step = 1;
            var labelStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = Color.white;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"{step++}. Set Scripting Define Symbols", labelStyle);
            {
                EditorGUILayout.BeginVertical("Box");
                defineSymbolsEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button($"{step++}. Build Windows64", EditorStyles.miniButtonLeft))
            {
                BuildUtils.BuildActiveScene(BuildTarget.StandaloneWindows64, "exe");
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
            EditorGUILayout.LabelField($"{step++}. Restore Scripting Define Symbols", labelStyle);
        }
    }
}
