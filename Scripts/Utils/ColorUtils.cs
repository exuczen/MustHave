using UnityEngine;
using System.Collections;

namespace MustHave.Utils
{
    public static class ColorExtensionMethods
    {
        public static int ToRGBA(this Color color)
        {
            Color32 color32 = color;
            return color32.ToRGBA();
        }

        public static int ToRGBA(this Color32 color)
        {
            int a = color.a << 0;
            int b = color.b << 8;
            int g = color.g << 16;
            int r = color.r << 24;
            int rgba = r | g | b | a;
            //Debug.Log($"From color: {ColorUtility.ToHtmlStringRGBA(color)} to RGBA: {rgba:X8}");
            return rgba;
        }
    }

    public struct ColorUtils
    {
        public static Color32 Color32FromRGBA(int rgba)
        {
            //int debugRGBA = rgba;
            byte a = (byte)(rgba >>= 0 & 0xff);
            byte b = (byte)(rgba >>= 8 & 0xff);
            byte g = (byte)(rgba >>= 8 & 0xff);
            byte r = (byte)(rgba >>= 8 & 0xff);
            //Debug.Log($"From RGBA: {debugRGBA:X8} to color: {ColorUtility.ToHtmlStringRGBA(new Color32(r, g, b, a))}");
            return new Color32(r, g, b, a);
        }

        public static Color32 Color32FromARGB(int argb)
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
            if (!hex.StartsWith("#"))
            {
                hex = string.Concat("#", hex);
            }
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }
    }
}
