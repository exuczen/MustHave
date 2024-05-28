using System;
using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public class MustHaveExporterWindow : EditorWindow
    {
        [SerializeField]
        private MustHaveExporterData exporterData = null;
        [SerializeField]
        private DefineSymbols editorDefineSymbols = null;
        [SerializeField]
        private DefineSymbols buildDefineSymbols = null;

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
            if (!exporterData)
            {
                return;
            }
            int step = 1;
            var labelStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = Color.white;

            scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition, false, true);

            Undo.RecordObject(exporterData, exporterData.name);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"{step++}. Set Scripting Define Symbols for build", labelStyle);
            {
                DefineSymbolsEditorWindow.OnDefineSymbolsGUI("Build", buildDefineSymbols, ref buildDefineSymbolsEditor);
            }
            if (GUILayout.Button($"{step++}. Build {exporterData.BuildTarget}"))
            {
                BuildUtils.BuildActiveScene(exporterData.BuildTarget);
            }
            // Build Target
            {
                OnExporterDataEnumGUI(() => exporterData.BuildTarget, value => exporterData.BuildTarget = value, "Build Target");
            }
            if (GUILayout.Button($"{step++}. Enable MustHave DLLs platforms for export"))
            {
                MustHaveExporter.EnableMustHaveLibsPlatforms();
            }
            if (GUILayout.Button($"{step++}. Export MustHave {exporterData.PackageName} Package"))
            {
                MustHaveExporter.ExportMustHavePackage(exporterData.PackageName);
            }
            // Package Name
            {
                OnExporterDataEnumGUI(() => exporterData.PackageName, value => exporterData.PackageName = value, "Package Name");
            }
            if (GUILayout.Button($"{step++}. Disable MustHave DLLs platforms"))
            {
                MustHaveExporter.DisableMustHaveLibsPlatforms();
            }
            EditorGUILayout.LabelField($"{step++}. Restore Scripting Define Symbols in editor", labelStyle);
            {
                DefineSymbolsEditorWindow.OnDefineSymbolsGUI("Editor", editorDefineSymbols, ref editorDefineSymbolsEditor);
            }
            AssetDatabase.SaveAssetIfDirty(exporterData);

            GUILayout.EndScrollView();
        }

        private void OnExporterDataEnumGUI<T>(Func<T> getEnumValue, Action<T> setEnumValue, string popupName) where T : Enum
        {
            EditorGUI.BeginChangeCheck();

            setEnumValue((T)EditorGUILayout.EnumPopup(popupName, getEnumValue()));

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(exporterData);
            }
        }
    }
}
