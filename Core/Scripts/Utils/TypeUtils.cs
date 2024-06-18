using System.Reflection;

namespace MustHave
{
    public readonly struct TypeUtils
    {
        public const BindingFlags InternalFieldFlags = BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic;
    }
}
