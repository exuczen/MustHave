using System;

namespace MustHave
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InactiveMenuItem : Attribute
    {
        public InactiveMenuItem(string itemName) { }
        public InactiveMenuItem(string itemName, bool isValidateFunction) { }
        public InactiveMenuItem(string itemName, bool isValidateFunction, int priority) { }
    }
}
