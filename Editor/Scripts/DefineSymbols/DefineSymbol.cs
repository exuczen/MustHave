using System;

namespace MustHave
{
    [Serializable]
    public struct DefineSymbol
    {
        public string name;
        public bool enabled;

        public DefineSymbol(string name, bool enabled)
        {
            this.enabled = enabled;
            this.name = name;
        }
    }
}
