using UnityEngine;

namespace MustHave.Utils
{
    public static class ComponentExtensionMethods
    {
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
            var component = thisComponent.GetComponent<T>();
            if (!component)
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
