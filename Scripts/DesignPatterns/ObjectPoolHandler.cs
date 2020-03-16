using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave.DesignPatterns
{
    public abstract class ObjectPoolHandler<T> : MonoBehaviour where T : PoolObject
    {
        [SerializeField]
        protected bool _fillPoolsOnAwake = true;
        [SerializeField]
        protected T[] _prefabs = default;
        [SerializeField]
        protected Transform[] _pools = default;
        [SerializeField]
        protected Transform[] _containers = default;

        public const int POOL_INIT_CAPACITY = 50;
        public const int POOL_DELTA_CAPACITY = 5;

        private void Awake()
        {
            if (_fillPoolsOnAwake)
            {
                for (int i = 0; i < _prefabs.Length; i++)
                {
                    FillPool(i, POOL_INIT_CAPACITY);
                }
            }
        }

        protected abstract void CreateObjectInstance(T prefab, Transform pool);

        protected void FillPool(int poolIndex, int count)
        {
            Transform pool = _pools[poolIndex];
            T prefab = _prefabs[poolIndex];
            for (int j = 0; j < count; j++)
            {
                CreateObjectInstance(prefab, pool);
            }
        }

        public T GetObjectFromPool(int poolIndex = 0)
        {
            Transform pool = _pools[poolIndex];
            Transform container = _containers[poolIndex];
            if (pool.childCount == 0)
            {
                FillPool(poolIndex, POOL_DELTA_CAPACITY);
            }
            T child = pool.GetChild(pool.childCount - 1).GetComponent<T>();
            child.transform.SetParent(container, false);
            child.gameObject.SetActive(true);
            return child;
        }

        public void ClearContainers()
        {
            for (int i = 0; i < _containers.Length; i++)
            {
                Transform pool = _pools[i];
                Transform container = _containers[i];
                Transform[] children = new Transform[container.childCount];
                for (int j = 0; j < container.childCount; j++)
                {
                    children[j] = container.GetChild(j);
                }
                foreach (Transform child in children)
                {
                    child.GetComponent<T>().MoveToPool(pool);
                }
            }
        }
    }
}
