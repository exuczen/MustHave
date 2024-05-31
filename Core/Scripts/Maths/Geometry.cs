using System;
using UnityEngine;

namespace MustHave
{
    public struct Geometry
    {
        public static Vector2 GetPositionOnCircle(float alfa, Vector2 center, float r)
        {
            float x = center.x + r * Mathf.Cos(alfa);
            float y = center.y + r * Mathf.Sin(alfa);
            return new Vector2(x, y);
        }

        public static void AddCirclePoints(Vector2[] points, int offset, Vector2 center, float r, int count, float begAngle = 0f)
        {
            float dalfa = 2f * MathF.PI / count;
            int index = offset;
            for (int i = 0; i < count; i++)
            {
                points[index++] = GetPositionOnCircle(begAngle + i * dalfa, center, r);
            }
        }

        public static int GetClosestPointIndex(Vector2 center, Vector2[] points, int beg, int end)
        {
            if (beg < 0 || end < 0)
            {
                return -1;
            }
            int pointIndex = beg;
            float sqrDistMin = (points[beg] - center).sqrMagnitude;
            for (int i = beg + 1; i <= end; i++)
            {
                float sqrDist = (points[i] - center).sqrMagnitude;
                if (sqrDist < sqrDistMin)
                {
                    sqrDistMin = sqrDist;
                    pointIndex = i;
                }
            }
            return pointIndex;
        }
    }
}
