using System.Reflection;

namespace MustHave
{
    public static class ObjectExtensionMethods
    {
        public static void SetFieldValue<T>(this T obj, string fieldName, object value, BindingFlags flags = TypeUtils.InternalFieldFlags)
        {
            var type = typeof(T);
            var fieldInfo = type.GetField(fieldName, flags);
            fieldInfo?.SetValue(obj, value);
        }

        public static void SetFieldValue(object obj, string fieldName, object value, BindingFlags flags = TypeUtils.InternalFieldFlags)
        {
            var type = obj.GetType();
            var fieldInfo = type.GetField(fieldName, flags);
            fieldInfo?.SetValue(obj, value);
        }
    }
}
