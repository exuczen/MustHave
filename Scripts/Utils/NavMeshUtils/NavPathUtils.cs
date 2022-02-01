using UnityEngine;
using UnityEngine.AI;

namespace MustHave
{
    public struct NavPathUtils
    {
        public static float GetPathLength(Vector3[] corners)
        {
            float length = 0f;
            for (int i = 1; i < corners.Length; i++)
            {
                length += Vector3.Distance(corners[i - 1], corners[i]);
            }
            return length;
        }

        public static float GetPathLength(NavMeshPath path)
        {
            return GetPathLength(path.corners);
        }
    }

    public static class NavPathExtensionMethods
    {
        public static Vector3 GetLastCorner(this NavMeshPath path)
        {
            var corners = path.corners;
            int count = corners.Length;
            return count > 0 ? corners[count - 1] : default;
        }

        public static float GetPathLength(this NavMeshPath path)
        {
            return NavPathUtils.GetPathLength(path);
        }
    } 
}
