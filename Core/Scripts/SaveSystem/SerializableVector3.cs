using UnityEngine;
using System;

namespace MustHave
{
    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        public override readonly string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", x, y, z);
        }

        public static implicit operator Vector3(SerializableVector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator SerializableVector3(Vector3 v)
        {
            return new SerializableVector3(v.x, v.y, v.z);
        }
    } 
}
