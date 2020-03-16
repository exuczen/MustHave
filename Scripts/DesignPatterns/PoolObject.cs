using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MustHave.DesignPatterns
{
    public abstract class PoolObject : MonoBehaviour
    {
        protected Transform _pool = default;

        protected abstract void OnMoveToPool();

        public void MoveToPool()
        {
            MoveToPool(_pool);
        }

        public void MoveToPool(Transform pool)
        {
            OnMoveToPool();
            transform.SetParent(pool, false);
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
            _pool = pool;
        }
    }
}
