using UnityEngine;

namespace MustHave.Utils
{
    public struct GeomUtils
    {
        public static Vector2 GetPositionOnCircle(float alfa, Vector2 center, float r)
        {
            float x = center.x + r * Mathf.Cos(alfa);
            float y = center.y + r * Mathf.Sin(alfa);
            return new Vector2(x, y);
        }
    }
}
