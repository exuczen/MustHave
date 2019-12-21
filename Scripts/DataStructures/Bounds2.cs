using System;
using UnityEngine;

namespace MustHave
{
    public struct Bounds2 : IEquatable<Bounds2>
    {
        private Vector2 size;
        private Vector2 extents;
        private Vector2 center;
        private Vector2 min;
        private Vector2 max;

        public Bounds2(Vector2 center, Vector2 size)
        {
            this.center = center;
            this.size = size;
            extents = size / 2f;
            min = center - extents;
            max = center + extents;
        }

        public Vector2 Size
        {
            get { return size; }
            set {
                size = value;
                extents = size / 2f;
                min = center - extents;
                max = center + extents;
            }
        }

        public Vector2 Extents
        {
            get { return extents; }
            set {
                extents = value;
                size = extents * 2f;
                min = center - extents;
                max = center + extents;
            }
        }

        public Vector2 Center
        {
            get { return center; }
            set {
                center = value;
                min = center - extents;
                max = center + extents;
            }
        }

        public Vector2 Min
        {
            get { return min; }
            set {
                min = value;
                size = max - min;
                extents = size / 2f;
                center = min + extents;
            }
        }

        public Vector2 Max
        {
            get { return max; }
            set {
                max = value;
                size = max - min;
                extents = size / 2f;
                center = max - extents;
            }
        }

        public void SetMinMax(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
            size = max - min;
            extents = size / 2f;
            center = max - extents;
        }

        public bool Contains(Vector2 pos)
        {
            float dx = pos.x - center.x;
            float dy = pos.y - center.y;
            return Mathf.Abs(dx) <= extents.x && Mathf.Abs(dy) <= extents.y;
        }

        public bool Equals(Bounds2 other)
        {
            return center == other.center && size == other.size;
        }

        public override string ToString()
        {
            return "center:" + center + " extents:" + extents;
        }

        public static Bounds2 BoundsToBounds2(Bounds bounds)
        {
            return new Bounds2(bounds.center, bounds.size);
        }
    }
}