using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public class DefineSymbolsEditorWindow : EditorWindow
    {
        [SerializeField]
        private DefineSymbols defineSymbols = null;

        private Editor editor = null;

        [MenuItem("Tools/Define Symbols Editor Window")]
        private static void ShowWindow()
        {
            GetWindow<DefineSymbolsEditorWindow>();
        }

        private void OnGUI()
        {
            var defineSymbolsType = typeof(DefineSymbols);
            defineSymbols = EditorGUILayout.ObjectField(defineSymbolsType.Name, defineSymbols, defineSymbolsType, true) as DefineSymbols;
            if (defineSymbols)
            {
                if (!editor)
                {
                    editor = Editor.CreateEditor(defineSymbols);
                }
                EditorGUILayout.Space();
                editor.OnInspectorGUI();
            }
        }
    }
}
