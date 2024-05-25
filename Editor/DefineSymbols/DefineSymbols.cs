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

        public void CopyFromArray(string[] names)
        {
            for (int i = 0; i < defineSymbols.Count; i++)
            {
                var symbol = defineSymbols[i];
                defineSymbols[i] = new DefineSymbol(symbol.name, false);
            }
            foreach (var name in names)
            {
                int index = defineSymbols.FindIndex(symbol => string.Equals(name, symbol.name));
                if (index >= 0)
                {
                    defineSymbols[index] = new DefineSymbol(name, true);
                }
                else
                {
                    defineSymbols.Add(new DefineSymbol(name, true));
                }
            }
        }
    }
}
