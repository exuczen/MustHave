using UnityEngine;
using System.Collections.Generic;

namespace MustHave
{
    public class DeprecatedObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        public const int DeltaCapacity = 5;

        public int ObjectCount => objectsUnused.Count + objectsInUse.Count;

        protected readonly List<T> objectsUnused = new();
        protected readonly HashSet<T> objectsInUse = new();
        protected readonly Transform parent = null;

        private readonly T prefab = null;

        public DeprecatedObjectPool(T prefab, Transform parent, int count)
        {
            this.parent = parent;
            this.prefab = prefab;

            for (int i = 0; i < count; i++)
            {
                CreateInstance();
            }
        }

        public T GetObject()
        {
            if (objectsUnused.Count == 0)
            {
                for (int i = 0; i < DeltaCapacity; i++)
                {
                    CreateInstance();
                }
            }

            T instance = objectsUnused.PickLastElement();
            if (!objectsInUse.Contains(instance))
            {
                objectsInUse.Add(instance);
            }
            return instance;
        }

        public void Return(T instance)
        {
            instance.OnReturnToPool();
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = prefab.transform.localScale;
            objectsInUse.Remove(instance);
            if (!objectsUnused.Contains(instance))
            {
                objectsUnused.Add(instance);
            }
        }

        public void SetAllInstancesScale(Vector3 localScale)
        {
            prefab.transform.localScale = localScale;
            foreach (var instance in objectsInUse)
            {
                instance.transform.localScale = localScale;
            }
            foreach (var instance in objectsUnused)
            {
                instance.transform.localScale = localScale;
            }
        }

        protected virtual void OnCreateInstance(T instance) { }

        protected T CreateInstance()
        {
            T objectInstance = GameObject.Instantiate(prefab, parent);
            objectInstance.transform.localScale = prefab.transform.localScale;
            objectInstance.gameObject.name = prefab.gameObject.name + objectInstance.gameObject.GetInstanceID();
            objectInstance.OnReturnToPool();
            OnCreateInstance(objectInstance);
            objectsUnused.Add(objectInstance);

            return objectInstance;
        }
    } 
}