using System;
using System.Reflection;

namespace MustHave
{
    public abstract class TypeInfo<T>
    {
        public const BindingFlags InternalFieldFlags = BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic;

        public static readonly Type Type = typeof(T);

        public static FieldInfo GetFieldInfo(string name, BindingFlags flags = InternalFieldFlags) => Type.GetField(name, flags);
    }
}
