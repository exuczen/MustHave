using System;

namespace MustHave
{
    [Serializable]
    public struct DefineSymbol
    {
        public bool enabled;
        public string name;

        public DefineSymbol(bool enabled, string name)
        {
            this.enabled = enabled;
            this.name = name;
        }
    }
}
