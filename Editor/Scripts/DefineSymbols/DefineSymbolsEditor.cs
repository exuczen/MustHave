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
            var symbols = target as DefineSymbols;

            Undo.RecordObject(symbols, symbols.name);

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(symbols);
            }
            EditorGUILayout.BeginVertical("Box");

            if (GUILayout.Button($"Get From {symbols.BuildTargetGroup}"))
            {
                PlayerSettings.GetScriptingDefineSymbols(symbols.BuildTarget, out string[] defines);

                symbols.CopyFromArray(defines);

                var serializedSymbols = new SerializedObject(symbols);
                serializedSymbols.ApplyModifiedProperties();

                EditorUtility.SetDirty(symbols);
            }
            if (GUILayout.Button($"Set For {symbols.BuildTargetGroup}"))
            {
                PlayerSettings.SetScriptingDefineSymbols(symbols.BuildTarget, symbols.GetEnabled());

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();

            AssetDatabase.SaveAssetIfDirty(symbols);
        }
    }
}
