using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MustHave.Utilities
{
    public struct ReflectionUtils
    {
        public static List<string> GetFieldsToStrings(Type type, object owner)
        {
            FieldInfo[] fieldInfos = type.GetFields();
            var list = new List<string>();
            foreach (var fieldInfo in fieldInfos)
            {
                list.Add(fieldInfo.Name + "=" + fieldInfo.GetValue(owner)?.ToString());
            }
            return list;
        }

        public static List<string> GetFieldsToStrings<T>(object owner)
        {
            return GetFieldsToStrings(typeof(T), owner);
        }

        /// <summary>
        /// Finds object of given type in owner properties
        /// </summary>
        /// <param name="type">Type of object to find in owner properties</param>
        /// <param name="owner">Owner of object to find</param>
        /// <returns></returns>
        public static object FindObjectOfTypeInOwnerProperties(Type type, object owner)
        {
            List<PropertyInfo> ownerProperties = owner.GetType().GetProperties().ToList();
            return FindObjectOfTypeInOwnerProperties(type, owner, ownerProperties);
        }

        /// <summary>
        /// Finds object of given type in owner properties
        /// </summary>
        /// <param name="type">Type of object to find in owner properties</param>
        /// <param name="owner">Owner of object to find</param>
        /// <param name="ownerProperties">Owner properties list</param>
        /// <returns></returns>
        public static object FindObjectOfTypeInOwnerProperties(Type type, object owner, List<PropertyInfo> ownerProperties)
        {
            PropertyInfo objectProperty = ownerProperties.Find(propertyInfo => propertyInfo.PropertyType == type);
            return objectProperty?.GetValue(owner, null);
        }

        /// <summary>
        /// Finds object of given type that derives from or implements interface of generic type T
        /// </summary>
        /// <typeparam name="T">Base class type</typeparam>
        /// <param name="type">Derived class type</param>
        /// <param name="owner">Owner of object property of derived class</param>
        /// <param name="ownerProperties">Owner properties list</param>
        /// <returns></returns>
        public static T FindObjectOfDerivedTypeInOwnerProperties<T>(Type type, object owner, List<PropertyInfo> ownerProperties) where T : UnityEngine.Object
        {
            object obj = FindObjectOfTypeInOwnerProperties(type, owner, ownerProperties);
            return obj != null && typeof(T).IsAssignableFrom(type) ? obj as T : null;
        }
    }
}
