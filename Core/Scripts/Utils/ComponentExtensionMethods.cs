using UnityEngine;

namespace MustHave.Utils
{
    public static class ComponentExtensionMethods
    {
        public static void DestroyMultipleComponentsInChilden<T>(this Component thisComponent, out T singleComponent) where T : Component
        {
            var components = thisComponent.GetComponentsInChildren<T>();
            if (components.Length > 0)
            {
                for (int i = 1; i < components.Length; i++)
                {
                    ObjectUtils.DestroyComponent(components[i]);
                }
                singleComponent = components[0];
            }
            else
            {
                singleComponent = thisComponent.gameObject.AddComponent<T>();
            }
        }

        public static void DestroyComponentsInChilden<T>(this Component thisComponent) where T : Component
        {
            var components = thisComponent.GetComponentsInChildren<T>();
            foreach (var cameraData in components)
            {
                ObjectUtils.DestroyComponent(cameraData);
            }
        }

        public static T GetOrAddComponent<T>(this Component thisComponent) where T : Component
        {
            if (!thisComponent.TryGetComponent<T>(out var component))
            {
                component = thisComponent.gameObject.AddComponent<T>();
            }
            return component;
        }

        public static void SetGameObjectActive(this Component component, bool active)
        {
            component.gameObject.SetActive(active);
        }

        public static bool IsGameObjectActiveSelf(this Component component)
        {
            return component.gameObject.activeSelf;
        }

        public static bool IsGameObjectActiveInHierarchy(this Component component)
        {
            return component.gameObject.activeInHierarchy;
        }
    }
}
