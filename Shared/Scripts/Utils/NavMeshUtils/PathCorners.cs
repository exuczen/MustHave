using UnityEngine;
using UnityEngine.AI;

namespace MustHave
{
    public class PathCorners
    {
        public int Count => corners.Length;
        public Vector3[] Corners { get => corners; set => corners = value; }
        public Vector3 FirstCorner => Count > 0 ? corners[0] : default;
        public Vector3 LastCorner => Count > 0 ? corners[Count - 1] : default;

        private Vector3[] corners = null;

        public PathCorners(NavMeshPath worldPath)
        {
            corners = worldPath.corners;
        }

        public PathCorners(Transform[] cornerPoints)
        {
            corners = new Vector3[cornerPoints.Length];
            for (int i = 0; i < cornerPoints.Length; i++)
            {
                corners[i] = cornerPoints[i].position;
            }
        }
    } 
}
