using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    public class LocalNavPath
    {
        public Vector3 StartDirection => CornersCount > 0 ? arcs[0].StartDirection : default;
        public Vector3 StartPoint => CornersCount > 0 ? localCorners[0] : default;
        public BezierArc[] BezierArcs => arcs;
        public float Length { get; private set; }
        public float BezierLength { get; private set; }
        public int CornersCount => localCorners.Length;
        public Vector3[] WorldCorners => worldCorners;
        public Vector3[] LocalCorners => localCorners;

        private readonly Vector3[] localCorners = null;
        private readonly Vector3[] worldCorners = null;

        private readonly BezierArc[] arcs = null;

        public LocalNavPath(Transform parent, params PathCorners[] worldPaths)
        {
            worldCorners = GetWorldCorners(parent.up, worldPaths);
            if (worldCorners.Length < 2)
            {
                Debug.LogError(GetType() + ": worldCorners.Length : " + worldCorners.Length);
                return;
            }
            localCorners = new Vector3[worldCorners.Length];

            for (int i = 0; i < worldCorners.Length; i++)
            {
                localCorners[i] = parent.InverseTransformPoint(worldCorners[i]);
            }
            arcs = BezierUtils.MakeBezierArcs(localCorners, EBezierType.Cubic);

            Length = NavPathUtils.GetPathLength(localCorners);
            if (arcs.Length > 0)
            {
                BezierLength = arcs[arcs.Length - 1].PathLength;
            }
        }

        public Vector3 GetLocalCorner(int cornerIndex)
        {
            return cornerIndex >= 0 && cornerIndex < CornersCount ? localCorners[cornerIndex] : default;
        }

        public Vector3 GetWorldCorner(int cornerIndex)
        {
            return cornerIndex >= 0 && cornerIndex < CornersCount ? worldCorners[cornerIndex] : default;
        }

        public Vector3 GetLastWorldCorner()
        {
            return worldCorners.Length > 0 ? worldCorners[worldCorners.Length - 1] : default;
        }

        public Vector3 GetBezierPointAtLength(float length)
        {
            int index = 0;
            while (index < arcs.Length && length > arcs[index].PathLength)
            {
                index++;
            }
            if (index < arcs.Length)
            {
                float deltaLength = index > 0 ? length - arcs[index - 1].PathLength : length;
                //Debug.Log(GetType() + ".GetBezierPointAtLength: " + index + " / " + CornersCount + " | " + length.ToString("f2") + " / " + arcs[index].PathLength.ToString("f2") + " | " + deltaLength.ToString("f2"));
                return arcs[index].GetPointAtLength(deltaLength);
            }
            else
            {
                return localCorners[CornersCount - 1];
            }
        }

        public void Draw(Transform parentTransform, Color color1, Color color2)
        {
            UpdateWorldCorners(parentTransform);

            var gizmosColor = Gizmos.color;
            Gizmos.color = color1;

            for (int i = 1; i < worldCorners.Length; i++)
            {
                Gizmos.DrawLine(worldCorners[i], worldCorners[i - 1]);
            }
#if UNITY_EDITOR
            //DrawBezierInEditor(color2);
#endif
            for (int i = 0; i < arcs.Length; i++)
            {
                arcs[i].Draw(parentTransform, color2);
            }
            Gizmos.color = gizmosColor;
        }

        public override string ToString()
        {
            return string.Format("{0}: Count: {1} Length: {2}", GetType(), CornersCount, Length);
        }

        private Vector3[] GetWorldCorners(Vector3 parentUp, PathCorners[] worldPaths)
        {
            if (worldPaths.Length < 1)
            {
                return null;
            }
            var allCorners = new List<Vector3>();
            var corners = worldPaths[0].Corners;
            if (corners.Length == 0)
            {
                Debug.LogError(GetType() + ".GetWorldCorners: " + corners.Length);
                return null;
            }

            Vector3 lastCorner = worldPaths[0].LastCorner;
            allCorners.AddRange(corners);

            for (int i = 1; i < worldPaths.Length; i++)
            {
                corners = worldPaths[i].Corners;
                if (corners.Length == 0)
                {
                    Debug.LogError(GetType() + ".GetWorldCorners: " + corners.Length);
                    return null;
                }
                allCorners.AddRange(corners);

                Vector3 cornersRay = corners[0] - lastCorner;
                float cornersUpLength = Vector3.Dot(cornersRay, parentUp);
                var cornersUpRay = cornersUpLength * parentUp;
                var cornersPlaneRay = cornersRay - cornersUpRay;
                if (Mathf.Abs(cornersUpLength) > 0.1f || cornersPlaneRay.sqrMagnitude < Mathv.Epsilon)
                {
                    allCorners.RemoveAt(allCorners.Count - corners.Length);
                }
                lastCorner = corners[corners.Length - 1];
            }
            return allCorners.ToArray();
        }

        private void UpdateWorldCorners(Transform parentTransform)
        {
            for (int i = 0; i < worldCorners.Length; i++)
            {
                worldCorners[i] = parentTransform.TransformPoint(localCorners[i]);
            }
        }

#if UNITY_EDITOR
        private void DrawBezierInEditor(Color color)
        {
            var corners = worldCorners;
            int count = corners.Length;
            Vector3 p0, p1, p2, p01, p12;

            for (int i = -1; i < worldCorners.Length - 1; i++)
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
                UnityEditor.Handles.DrawBezier(p01, p12, p1, p1, color, null, 5f);
            }
        }
#endif
    } 
}
