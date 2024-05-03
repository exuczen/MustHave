using System;
using UnityEngine;

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

        public int this[int index]
        {
            get
            {
                if (index >= 0 && index < 3)
                {
                    return xyz[index];
                }
                else if (index == 3)
                {
                    return w;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                if (index >= 0 && index < 3)
                {
                    xyz[index] = value;
                }
                else if (index == 3)
                {
                    w = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public override readonly string ToString()
        {
            return $"{xyz} {w}";
        }
    }
}
