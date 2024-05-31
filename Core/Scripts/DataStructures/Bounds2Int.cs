using System;
using UnityEngine;

namespace MustHave
{
    public struct Bounds2Int : IEquatable<Bounds2Int>
    {
        private Vector2Int size;
        private Vector2Int min;
        private Vector2Int max;

        public Bounds2Int(Vector2Int min, Vector2Int max)
        {
            this.min = min;
            this.max = max;
            size = max - min + Vector2Int.one;
        }

        public Vector2Int Size
        {
            get { return size; }
            set
            {
                size = value;
                max = min + size - Vector2Int.one;
            }
        }

        public Vector2Int Min
        {
            get { return min; }
            set
            {
                min = value;
                size = max - min + Vector2Int.one;
            }
        }

        public Vector2Int Max
        {
            get { return max; }
            set
            {
                max = value;
                size = max - min + Vector2Int.one;
            }
        }

        public bool Overlaps(Bounds2Int other)
        {
            return other.min.x <= max.x && other.max.x >= min.x
                && other.min.y <= max.y && other.max.y >= min.y;
        }

        public bool Contains(Vector2Int pos)
        {
            return pos.x >= min.x && pos.x <= max.x
                && pos.y >= min.y && pos.y <= max.y;
        }

        public bool Contains(Vector2 pos)
        {
            return pos.x >= min.x && pos.x <= max.x
                && pos.y >= min.y && pos.y <= max.y;
        }

        public bool Equals(Bounds2Int other)
        {
            return min == other.min && max == other.max;
        }
    }
}