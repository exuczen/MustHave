using MustHave.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MustHave
{
    [CustomEditor(typeof(DefineSymbols))]
    public class DefineSymbolsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var symbols = target as DefineSymbols;

            EditorGUILayout.BeginVertical("Box");

            if (GUILayout.Button("Get From Standalone"))
            {
                PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone, out string[] defines);

                symbols.SetFromArray(defines);
            }
            if (GUILayout.Button("Set For Standalone"))
            {
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, symbols.GetEnabled());
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
