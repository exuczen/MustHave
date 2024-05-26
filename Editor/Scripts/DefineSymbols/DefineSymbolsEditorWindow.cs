using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public class DefineSymbolsEditorWindow : EditorWindow
    {
        [SerializeField]
        private DefineSymbols editorDefineSymbols = null;
        [SerializeField]
        private DefineSymbols standaloneDefineSymbols = null;

        private Editor editorDefineSymbolsEditor = null;
        private Editor standaloneDefineSymbolsEditor = null;

        private Vector2 scrollViewPosition = default;

        public static void OnDefineSymbolsGUI(string prefix, DefineSymbols symbols, ref Editor editor)
        {
            EditorGUILayout.BeginVertical("Box");
            {
                var symbolsType = typeof(DefineSymbols);
                symbols = EditorGUILayout.ObjectField($"{prefix}{symbolsType.Name}", symbols, symbolsType, true) as DefineSymbols;
                if (symbols)
                {
                    if (!editor)
                    {
                        editor = Editor.CreateEditor(symbols);
                    }
                    editor.OnInspectorGUI();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        [MenuItem("Tools/MustHave/Scripting Define Symbols")]
        private static void ShowWindow()
        {
            GetWindow<DefineSymbolsEditorWindow>();
        }

        private void OnGUI()
        {
            scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition, false, true);

            OnDefineSymbolsGUI("Editor", editorDefineSymbols, ref editorDefineSymbolsEditor);
            OnDefineSymbolsGUI("Standalone", standaloneDefineSymbols, ref standaloneDefineSymbolsEditor);

            GUILayout.EndScrollView();
        }
    }
}
