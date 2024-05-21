﻿using UnityEngine;

namespace MustHave
{
    public struct TexArrayColor
    {
        public float Alpha { get => Color.a; set => color.a = value; }
        public Color Color { get => color; set => color = value; }
        public int TexIndex { get => texIndex; set => texIndex = value; }

        private Color color;
        private int texIndex;

        public static bool operator ==(TexArrayColor color1, TexArrayColor color2)
        {
            return color1.Equals(color2);
        }

        public static bool operator !=(TexArrayColor color1, TexArrayColor color2)
        {
            return !color1.Equals(color2);
        }

        public static int AlphaToTextureIndex(float alpha, int texArrayLength)
        {
            return (int)(alpha * texArrayLength - 0.49f);
        }

        public static float TextureIndexToAlpha(int texIndex, int texArrayLength)
        {
            return 1f * (texIndex + 0.5f) / texArrayLength;
        }

        public TexArrayColor(Color color, int texIndex)
        {
            this.texIndex = texIndex;
            this.color = color;
        }

        public TexArrayColor(TexArrayColor texColor, float alpha) : this(texColor.Color, texColor.TexIndex)
        {
            color.a = alpha;
        }

        public override readonly bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return color.ToString() + " texIndex: " + texIndex;
        }

        public bool Equals(TexArrayColor other)
        {
            return Color == other.Color && TexIndex == other.TexIndex;
        }

        public Color GetControlColor(int texArrayLength)
        {
            Color color = Color.white;
            color.a = TextureIndexToAlpha(TexIndex, texArrayLength);
            return color;
        }
    }
}
