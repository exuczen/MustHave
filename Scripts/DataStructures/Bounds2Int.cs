using System;
using UnityEngine;

namespace MustHave
{
    public struct Bounds2Int : IEquatable<Bounds2Int>
    {
        private Vector2Int size;
        private Vector2Int min;
        private Vector2Int max;

        public Bounds2Int(Vector2Int min, Vector2Int size)
        {
            this.min = min;
            this.max = min + size;
            this.size = size;
        }

        public Bounds2Int GetScaledBounds(int bitShift)
        {
            Bounds2Int bounds;
            if (bitShift == 0)
            {
                return this;
            }
            else if (bitShift < 0)
            {
                bitShift = -bitShift;
                bounds = new Bounds2Int {
                    size = new Vector2Int(size.x >> bitShift, size.y >> bitShift),
                    min = new Vector2Int(min.x >> bitShift, min.y >> bitShift),
                    max = new Vector2Int(max.x >> bitShift, max.y >> bitShift)
                };
            }
            else
            {
                bounds = new Bounds2Int {
                    size = new Vector2Int(size.x << bitShift, size.y << bitShift),
                    min = new Vector2Int(min.x << bitShift, min.y << bitShift),
                    max = new Vector2Int(max.x << bitShift, max.y << bitShift)
                };
            }
            return bounds;
        }

        public Vector2Int Size
        {
            get { return size; }
            set {
                size = value;
                max = min + size;
            }
        }

        public Vector2Int Min
        {
            get { return min; }
            set {
                min = value;
                size = max - min;
            }
        }

        public Vector2Int Max
        {
            get { return max; }
            set {
                max = value;
                size = max - min;
            }
        }

        public bool Contains(Vector2Int pos)
        {
            int dx = pos.x - min.x;
            int dy = pos.y - min.y;
            return dx >= 0 && dx <= size.x && dy >= 0 && dy <= size.y;
        }

        public bool Contains(Vector2 pos)
        {
            float dx = pos.x - min.x;
            float dy = pos.y - min.y;
            return dx >= 0f && dx <= size.x && dy >= 0f && dy <= size.y;
        }


        public bool Equals(Bounds2Int other)
        {
            return min == other.min && max == other.max;
        }
    }
}