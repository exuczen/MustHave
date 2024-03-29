﻿using System;
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

        public static implicit operator Bounds(Bounds2 bounds)
        {
            return new Bounds(bounds.center, bounds.size);
        }

        public static implicit operator Bounds2(Bounds bounds)
        {
            return new Bounds2(bounds.center, bounds.size);
        }

        public static Bounds2 BoundsToBounds2(Bounds bounds)
        {
            return new Bounds2(bounds.center, bounds.size);
        }

        public static Bounds2 MinMax(Vector2 min, Vector2 max)
        {
            Bounds2 bounds = new Bounds2();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public Bounds2(Vector2 center, Vector2 size)
        {
            this.center = center;
            this.size = size;
            extents = size * 0.5f;
            min = center - extents;
            max = center + extents;
        }

        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                extents = size * 0.5f;
                min = center - extents;
                max = center + extents;
            }
        }

        public Vector2 Extents
        {
            get { return extents; }
            set
            {
                extents = value;
                size = extents * 0.5f;
                min = center - extents;
                max = center + extents;
            }
        }

        public Vector2 Center
        {
            get { return center; }
            set
            {
                center = value;
                min = center - extents;
                max = center + extents;
            }
        }

        public Vector2 Min
        {
            get { return min; }
            set
            {
                min = value;
                size = max - min;
                extents = size * 0.5f;
                center = min + extents;
            }
        }

        public Vector2 Max
        {
            get { return max; }
            set
            {
                max = value;
                size = max - min;
                extents = size * 0.5f;
                center = min + extents;
            }
        }

        public void SetMinMax(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
            size = max - min;
            extents = size * 0.5f;
            center = min + extents;
        }

        public bool Overlaps(Bounds2 other)
        {
            return other.min.x <= max.x && other.max.x >= min.x
                && other.min.y <= max.y && other.max.y >= min.y;
        }

        public bool Contains(Vector2 pos)
        {
            return pos.x >= min.x && pos.x <= max.x
                && pos.y >= min.y && pos.y <= max.y;
        }

        public bool Equals(Bounds2 other)
        {
            return center == other.center && size == other.size;
        }

        public override string ToString()
        {
            return "center:" + center + " extents:" + extents;
        }
    }
}