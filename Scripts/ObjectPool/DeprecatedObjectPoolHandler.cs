using UnityEngine;

namespace MustHave
{
    public abstract class DeprecatedObjectPoolHandler<T> : MonoBehaviour where T : DeprecatedPoolObject
    {
        [SerializeField]
        protected bool fillPoolsOnAwake = true;
        [SerializeField]
        protected T[] prefabs = default;
        [SerializeField]
        protected Transform[] pools = default;
        [SerializeField]
        protected Transform[] containers = default;

        public const int POOL_INIT_CAPACITY = 50;
        public const int POOL_DELTA_CAPACITY = 5;

        private void Awake()
        {
            if (fillPoolsOnAwake)
            {
                for (int i = 0; i < prefabs.Length; i++)
                {
                    FillPool(i, POOL_INIT_CAPACITY);
                }
            }
        }

        protected abstract void CreateObjectInstance(T prefab, Transform pool);

        protected void FillPool(int poolIndex, int count)
        {
            Transform pool = pools[poolIndex];
            T prefab = prefabs[poolIndex];
            for (int j = 0; j < count; j++)
            {
                CreateObjectInstance(prefab, pool);
            }
        }

        public T GetObjectFromPool(int poolIndex = 0)
        {
            Transform pool = pools[poolIndex];
            Transform container = containers[poolIndex];
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
            for (int i = 0; i < containers.Length; i++)
            {
                Transform pool = pools[i];
                Transform container = containers[i];
                Transform[] children = new Transform[container.childCount];
                for (int j = 0; j < container.childCount; j++)
                {
                    children[j] = container.GetChild(j);
                }
                foreach (Transform child in children)
                {
                    child.GetComponent<T>().ReturnToPool(pool);
                }
            }
        }
    }
}
