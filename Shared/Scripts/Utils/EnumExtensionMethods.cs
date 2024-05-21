using System;
using System.Collections.Generic;
using System.Linq;

namespace MustHave.Utils
{
    public static class EnumExtensionMethods
    {
        //public static int EnumToInt(this Enum value) => Convert.ToInt32(value);
        public static int EnumToInt(this Enum value) => (int)(object)value;

        public static int EnumToInt<T>(this T value) where T : Enum => (int)(object)value;

        public static IEnumerable<string> CastToStringEnumerable(this Enum[] array)
        {
            return array.Select(enumName => enumName.ToString());
        }

        public static List<string> CastToStringList(this Enum[] array)
        {
            return CastToStringEnumerable(array).ToList();
        }
    }
}
