using UnityEngine;

namespace MustHave
{
    public static class TypeConverter
    {
        public static short FloatToShort(float value, float max)
        {
            value = Mathf.Clamp01(value / max) * short.MaxValue;
            return (short)value;
        }

        public static float ShortToFloat(short value, float max)
        {
            return value * max / short.MaxValue;
        }
    }
}
