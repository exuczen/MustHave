using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [RequireComponent(typeof(Canvas))]
    public class PersistentCanvas<T> : PersistentSingleton<T> where T : MonoBehaviour
    {
        [SerializeField] protected List<Object> persistentObjects = new List<Object>();
        [SerializeField] protected List<Component> persistentComponents = new List<Component>();

        public void ClearPersistentComponentsList()
        {
            persistentComponents.Clear();
        }

        public void ClearPersistentObjectsList()
        {
            persistentObjects.Clear();
        }

        public void AddPersistentObjectsToList(params Object[] objects)
        {
            AddPersistentElementsToList(persistentObjects, objects);
        }

        public void AddPersistentComponentsToList(params Component[] components)
        {
            AddPersistentElementsToList(persistentComponents, components);
        }

        public void SetPersistentComponentsList(params Component[] components)
        {
            ClearPersistentComponentsList();
            AddPersistentComponentsToList(components);
        }

        public static void AddPersistentElementsToList<T1>(List<T1> list, params T1[] components)
        {
            foreach (var component in components)
            {
                if (!list.Contains(component))
                {
                    list.Add(component);
                }
            }
        }

        public void RemovePersistentObjectFromList(Object obj)
        {
            persistentObjects.Remove(obj);
        }

        public void RemovePersistentObjectFromList<T1>(string name) where T1 : Object
        {
            T1 obj = GetPersistentObjectOfTypeByName<T1>(name);
            if (obj)
                persistentObjects.Remove(obj);
        }

        public void SetPersistentComponentsParent(Transform parent)
        {
            persistentComponents.ForEach(component => component.transform.SetParent(parent, false));
        }

        public Component GetFirstPersistentComponent()
        {
            return persistentComponents.Count > 0 ? persistentComponents[0] : null;
        }

        public T1 GetPersistentComponentOfType<T1>() where T1 : Component
        {
            return persistentComponents.Find(component => (component is T1)) as T1;
        }

        public T1 GetPersistentObjectOfTypeByName<T1>(string name, bool removeFromList) where T1 : Object
        {
            T1 obj = GetPersistentObjectOfTypeByName<T1>(name);
            if (removeFromList && obj)
                persistentObjects.Remove(obj);
            return obj;
        }

        private T1 GetPersistentObjectOfTypeByName<T1>(string name) where T1 : Object
        {
            return persistentObjects.Find(obj => obj && (obj is T1) && obj.name.Equals(name)) as T1;
        }
    }
}
