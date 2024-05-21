using UnityEngine;

namespace MustHave
{
    public class BezierArc
    {
        public Vector3 StartCorner => corners[0];
        public Vector3 StartPoint => points[0].Position;
        public Vector3 EndPoint => points[PointsCount - 1].Position;
        public Vector3 StartDirection => startDirection;
        public int PointsCount => points.Length;
        public float Length { get; }
        public float PathLength { get; private set; }

        private readonly BezierPoint[] points = null;
        private readonly Vector3[] corners = null;

        private Vector3 startDirection = default;

        public BezierArc(float begPathLength, params Vector3[] corners)
        {
            if (corners.Length < 3 || corners.Length > 4)
            {
                Debug.LogError(GetType() + " corners.Length: " + corners.Length);
                return;
            }
            this.corners = corners;
            startDirection = corners[1] - corners[0];
            if (startDirection.sqrMagnitude < Mathv.Epsilon)
            {
                startDirection = corners[corners.Length - 1] - corners[0];
            }
            points = BezierUtils.MakeBezierPoints(out float length, corners);
            Length = length;
            PathLength = begPathLength + length;
        }

        public void Draw(Transform parentTransform, Color color)
        {
            var gizmosColor = Gizmos.color;
            Gizmos.color = color;

            Vector3 prevPos = parentTransform.TransformPoint(points[0].Position);
            for (int i = 1; i < points.Length; i++)
            {
                Vector3 currPos = parentTransform.TransformPoint(points[i].Position);
                Gizmos.DrawLine(prevPos, currPos);
                prevPos = currPos;
            }

            Gizmos.color = gizmosColor;
        }

        public Vector3 GetPointAtLength(float length)
        {
            int index = 0;
            while (index < points.Length && length > points[index].ArcLength)
            {
                index++;
            }
            if (index > 0 && index < points.Length)
            {
                var prevPoint = points[index - 1];
                var nextPoint = points[index];
                float deltaLength = length - prevPoint.ArcLength;
                float prevNextLength = nextPoint.ArcLength - prevPoint.ArcLength;
                float deltaTransition = deltaLength / prevNextLength;

                //float bezierTransition = Mathf.Lerp(prevPoint.Transition, nextPoint.Transition, deltaTransition);
                //Debug.Log(GetType() + ".GetPointAtLength: " + index + " / " + PointsCount + " | " + length.ToString("f2") + " / " + Length.ToString("f2") + " | " + bezierTransition.ToString("f2"));
                //return BezierUtils.GetBezierPoint(bezierTransition, corners);

                return Vector3.Lerp(prevPoint.Position, nextPoint.Position, deltaTransition);
            }
            else
            {
                //Debug.Log(GetType() + ".GetPointAtLength: " + length.ToString("f2") + " / " + Length.ToString("f2"));
                return points[Mathf.Clamp(index, 0, points.Length - 1)].Position;
            }
        }
    } 
}
