using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave.DesignPatterns
{
    [RequireComponent(typeof(Canvas))]
    public class PersistentCanvas<T> : PersistentSingleton<T> where T : MonoBehaviour
    {
        [SerializeField] protected List<Object> _persistentObjectsList = new List<Object>();
        [SerializeField] protected List<Component> _persistentComponentsList = new List<Component>();

        public void ClearPersistentComponentsList()
        {
            _persistentComponentsList.Clear();
        }

        public void ClearPersistentObjectsList()
        {
            _persistentObjectsList.Clear();
        }

        public void AddPersistentObjectsToList(params Object[] objects)
        {
            AddPersistentElementsToList(_persistentObjectsList, objects);
        }

        public void AddPersistentComponentsToList(params Component[] components)
        {
            AddPersistentElementsToList(_persistentComponentsList, components);
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
            _persistentObjectsList.Remove(obj);
        }

        public void RemovePersistentObjectFromList<T1>(string name) where T1 : Object
        {
            T1 obj = GetPersistentObjectOfTypeByName<T1>(name);
            if (obj)
                _persistentObjectsList.Remove(obj);
        }

        public void SetPersistentComponentsParent(Transform parent)
        {
            _persistentComponentsList.ForEach(component => component.transform.SetParent(parent, false));
        }

        public Component GetFirstPersistentComponent()
        {
            return _persistentComponentsList.Count > 0 ? _persistentComponentsList[0] : null;
        }

        public T1 GetPersistentComponentOfType<T1>() where T1 : Component
        {
            return _persistentComponentsList.Find(component => (component is T1)) as T1;
        }

        public T1 GetPersistentObjectOfTypeByName<T1>(string name, bool removeFromList) where T1 : Object
        {
            T1 obj = GetPersistentObjectOfTypeByName<T1>(name);
            if (removeFromList && obj)
                _persistentObjectsList.Remove(obj);
            return obj;
        }

        private T1 GetPersistentObjectOfTypeByName<T1>(string name) where T1 : Object
        {
            return _persistentObjectsList.Find(obj => obj && (obj is T1) && obj.name.Equals(name)) as T1;
        }
    }
}
