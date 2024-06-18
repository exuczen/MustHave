using UnityEngine;
using System.Collections.Generic;

namespace MustHave
{
    public class SerializedObjectPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        public int ObjectCount => unusedObjects.Count + objectsInUse.Count;

        [SerializeField]
        private T prefab = null;
        [SerializeField]
        private List<T> unusedObjects = new();
        [SerializeField]
        private int initialCapacity = 10;
        [SerializeField]
        private int deltaCapacity = 5;

        private readonly HashSet<T> objectsInUse = new HashSet<T>();

        public void ClearPool()
        {
            if (Application.isPlaying)
            {
                transform.DestroyAllChildren();
            }
#if UNITY_EDITOR
            else
            {
                transform.DestroyAllChildrenImmediate();
            }
#endif
            unusedObjects.Clear();
        }

        public void FillPool()
        {
            for (int i = 0; i < initialCapacity; i++)
            {
                CreateInstance();
            }
        }

        public T GetObject()
        {
            if (unusedObjects.Count == 0)
            {
                for (int i = 0; i < deltaCapacity; i++)
                {
                    CreateInstance();
                }
            }

            T instance = unusedObjects.PickLastElement();
            if (!objectsInUse.Contains(instance))
            {
                objectsInUse.Add(instance);
            }
            return instance;
        }

        public void Return(T instance)
        {
            instance.OnReturnToPool();
            instance.transform.SetParent(transform, false);
            instance.transform.localScale = prefab.transform.localScale;
            objectsInUse.Remove(instance);
            if (!unusedObjects.Contains(instance))
            {
                unusedObjects.Add(instance);
            }
        }

        public void SetAllInstancesScale(Vector3 localScale)
        {
            prefab.transform.localScale = localScale;
            foreach (var instance in objectsInUse)
            {
                instance.transform.localScale = localScale;
            }
            foreach (var instance in unusedObjects)
            {
                instance.transform.localScale = localScale;
            }
        }

        protected virtual void OnCreateInstance(T instance) { }

        protected T CreateInstance()
        {
            T objectInstance = Instantiate(prefab, transform);
            objectInstance.transform.localScale = prefab.transform.localScale;
            objectInstance.gameObject.name = prefab.gameObject.name + objectInstance.gameObject.GetInstanceID();
            objectInstance.OnReturnToPool();
            OnCreateInstance(objectInstance);
            unusedObjects.Add(objectInstance);

            return objectInstance;
        }
    } 
}