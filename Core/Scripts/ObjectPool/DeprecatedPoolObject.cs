using UnityEngine;

namespace MustHave
{
    public abstract class DeprecatedPoolObject : MonoBehaviour
    {
        protected Transform pool = default;

        protected abstract void OnReturnToPool();

        public void ReturnToPool()
        {
            ReturnToPool(pool);
        }

        public void ReturnToPool(Transform pool)
        {
            OnReturnToPool();
            transform.SetParent(pool, false);
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
            this.pool = pool;
        }
    }
}
