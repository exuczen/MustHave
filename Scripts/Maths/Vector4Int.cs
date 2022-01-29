﻿using UnityEngine;

namespace MustHave
{
    public struct Vector4Int
    {
        public Vector3Int xyz;
        public int w;

        public Vector4Int(Vector3Int xyz, int w)
        {
            this.xyz = xyz;
            this.w = w;
        }

        public override string ToString()
        {
            return xyz.ToString() + " " + w;
        }
    }
}
