using System;
using System.Reflection;

namespace MustHave
{
    public abstract class TypeInfo<T>
    {
        public static readonly Type Type = typeof(T);

        public static FieldInfo GetFieldInfo(string name, BindingFlags flags = TypeUtils.InternalFieldFlags) => Type.GetField(name, flags);
    }
}
