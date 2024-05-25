using MustHave.UI;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(fileName = "ScriptingDefineSymbols", menuName = "MustHave/ScriptableObjects/ScriptingDefineSymbols")]
    public class DefineSymbols : ScriptableObject
    {
        [SerializeField]
        [ArrayElementTitle("name")]
        private List<DefineSymbol> defineSymbols = new();

        public string[] GetEnabled()
        {
            var enabled = defineSymbols.FindAll(symbol => symbol.enabled);
            var enabledNames = new string[enabled.Count];
            for (int i = 0; i < enabled.Count; i++)
            {
                enabledNames[i] = enabled[i].name;
            }
            return enabledNames;
        }

        public void SetFromArray(string[] defines)
        {
            defineSymbols.Clear();
            foreach (var name in defines)
            {
                defineSymbols.Add(new DefineSymbol(true, name));
            }
        }
    }
}
