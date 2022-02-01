using UnityEngine;

namespace MustHave
{
    public struct BezierPoint
    {
        public Vector3 Position { get; private set; }
        public float Transition { get; private set; }
        public float ArcLength { get; private set; }

        public BezierPoint(Vector3 position, float transition, float arcLength)
        {
            Position = position;
            Transition = transition;
            ArcLength = arcLength;
        }

        public BezierPoint(Vector3 p0)
        {
            Position = p0;
            Transition = 0f;
            ArcLength = 0f;
        }
    } 
}
