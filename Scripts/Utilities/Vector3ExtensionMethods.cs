using UnityEngine;

namespace MustHave.Utilities
{
    public static class Vector3ExtensionMethods
    {
        public static Vector3 Modulo(this Vector3 v, float modulo)
        {
            return new Vector3(
                v.x % modulo,
                v.y % modulo,
                v.z % modulo
                );
        }
    }
}
