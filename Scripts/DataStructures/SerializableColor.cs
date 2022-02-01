using System;
using UnityEngine;

namespace MustHave
{
    [Serializable]
    public struct SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public SerializableColor(Color color, float a) : this(color.r, color.g, color.b, a) { }

        public SerializableColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}]", r, g, b, a);
        }

        public static implicit operator Color(SerializableColor color)
        {
            return new Color(color.r, color.g, color.b, color.a);
        }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color.r, color.g, color.b, color.a);
        }

        public static bool operator ==(SerializableColor color1, SerializableColor color2)
        {
            return color1.Equals(color2);
        }

        public static bool operator !=(SerializableColor color1, SerializableColor color2)
        {
            return !color1.Equals(color2);
        }

        public static bool operator ==(Color color1, SerializableColor color2)
        {
            return color1.Equals(color2);
        }

        public static bool operator !=(Color color1, SerializableColor color2)
        {
            return !color1.Equals(color2);
        }

        public static bool operator ==(SerializableColor color2, Color color1)
        {
            return color1.Equals(color2);
        }

        public static bool operator !=(SerializableColor color2, Color color1)
        {
            return !color1.Equals(color2);
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    } 
}
