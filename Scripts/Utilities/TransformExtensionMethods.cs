using System.Collections.Generic;
using UnityEngine;

namespace MustHave.Utilities
{
    public static class TransformExtensionMethods
    {
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
            transform.DetachChildren();
        }

        public static void DestroyAllChildrenImmediate(this Transform transform)
        {
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i).transform);
            }
            foreach (Transform child in children)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
            transform.DetachChildren();
        }

        public static T GetComponentInParents<T>(this Transform transform) where T : Component
        {
            Transform parent = transform.parent;
            T component = null;
            while (parent && component == null)
            {
                component = parent.GetComponent<T>();
                parent = parent.parent;
            }
            return component;
        }
    }
}
