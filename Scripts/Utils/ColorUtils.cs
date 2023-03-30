using UnityEngine;
using System.Collections;

namespace MustHave.Utils
{
    public struct ColorUtils
    {
        public static Color32 ColorFromARGB(int argb)
        {
            byte b = (byte)(argb >>= 0 & 0xff);
            byte g = (byte)(argb >>= 8 & 0xff);
            byte r = (byte)(argb >>= 8 & 0xff);
            byte a = (byte)(argb >>= 8 & 0xff);
            return new Color32(r, g, b, a);
        }

        public static Color ColorWithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color ColorWithScaledBrightness(Color color, float brightnessScale)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return Color.HSVToRGB(h, s, v * brightnessScale);
        }

        public static string ColorToHexNoAlphaNoHash(Color32 color)
        {
            return string.Concat(color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        private static string ColorToHexNoAlpha(Color32 color)
        {
            return string.Concat("#", color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        private static string ColorToHex(Color32 color)
        {
            return string.Concat(ColorToHexNoAlpha(color), color.a.ToString("X2"));
        }

        public static string ColorToHex(Color color, float alpha)
        {
            color.a = alpha;
            return ColorToHex(color);
        }

        public static Color HexToColor(string hex)
        {
            Color color = Color.white;
            if (!hex.StartsWith("#"))
                hex = string.Concat("#", hex);
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}
