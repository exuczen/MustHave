using UnityEngine;

namespace MustHave
{
    public struct BezierUtils
    {
        public static BezierArc[] MakeBezierArcs(Vector3[] corners, EBezierType type)
        {
            int count = corners.Length;
            if (count < 2)
            {
                Debug.LogError("BezierUtils.MakeBezierArcs: count: " + count);
                return null;
            }
            var arcs = new BezierArc[count];
            bool cubic = type == EBezierType.Cubic;
            float pathLength = 0f;

            Vector3 p0, p1, p2, p01, p12;
            for (int i = -1; i < count - 1; i++)
            {
                if (i < 0)
                {
                    p01 = corners[0];
                    p12 = (corners[0] + corners[1]) * 0.5f;
                    p1 = (p01 + p12) * 0.5f;
                }
                else if (i < count - 2)
                {
                    p0 = corners[i];
                    p1 = corners[i + 1];
                    p2 = corners[i + 2];
                    p01 = (p0 + p1) * 0.5f;
                    p12 = (p1 + p2) * 0.5f;
                }
                else
                {
                    p01 = (corners[count - 2] + corners[count - 1]) * 0.5f;
                    p12 = corners[count - 1];
                    p1 = (p01 + p12) * 0.5f;
                }
                var arc = cubic ? new BezierArc(pathLength, p01, p1, p1, p12) : new BezierArc(pathLength, p01, p1, p12); ;
                arcs[i + 1] = arc;
                pathLength = arc.PathLength;

                //Debug.Log("BezierUtils.MakeBezierArcs: " + i + " | " + arc.Length.ToString("f2") + " | " + arc.PathLength.ToString("f2"));
            }
            return arcs;
        }

        public static BezierPoint[] MakeBezierPoints(out float length, params Vector3[] corners)
        {
            if (corners.Length < 3 || corners.Length > 4)
            {
                Debug.LogError("MakeBezierPoints: corners.Length: " + corners.Length);
                length = 0f;
                return null;
            }
            Vector3 dir1 = corners[1] - corners[0];
            Vector3 dir2 = corners[corners.Length - 1] - corners[corners.Length - 2];
            float angle = Mathf.Abs(Vector3.Angle(dir1, dir2));
            float estimatedLength = GetEstimatedArcLenght(corners);
            int count = 3 + (int)(Mathf.Max(1f, 3f * estimatedLength) * angle / 5f);
            var points = new BezierPoint[count];
            points[0] = new BezierPoint(corners[0]);
            length = 0f;

            for (int i = 1; i < count; i++)
            {
                float t = 1f * i / (count - 1);
                var prevPos = points[i - 1].Position;
                var currPos = GetBezierPoint(t, corners);
                length += Vector3.Distance(currPos, prevPos);
                points[i] = new BezierPoint(currPos, t, length);
                //Debug.Log("MakeBezierPoints: " + i + " " + points[i].Position.ToString("f2"));
            }

            return points;
        }

        public static float GetEstimatedArcLenght(params Vector3[] corners)
        {
            int count = corners.Length;
            if (count < 2)
            {
                return 0f;
            }
            float chord = Vector3.Distance(corners[count - 1], corners[0]);
            float lineLength = NavPathUtils.GetPathLength(corners);
            return (chord + lineLength) * 0.5f;

            //float totalLength = chord + lineLength;
            //return chord * chord / totalLength + lineLength * lineLength / totalLength;
        }

        public static Vector3 GetBezierPoint(float t, params Vector3[] pts)
        {
            if (pts.Length == 4)
            {
                return GetBezierPoint(t, pts[0], pts[1], pts[2], pts[3]);
            }
            else if (pts.Length == 3)
            {
                return GetBezierPoint(t, pts[0], pts[1], pts[2]);
            }
            else
            {
                Debug.LogError("BezierUtils.GetBezierPoint: pts.Length:" + pts.Length);
                return default;
            }
        }

        public static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2f * u * t * p1;
            p += tt * p2;

            return p;
        }

        public static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3f * uu * t * p1;
            p += 3f * u * tt * p2;
            p += ttt * p3;

            return p;
        }
    } 
}
