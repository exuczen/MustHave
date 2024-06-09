using MustHave.Utils;
using UnityEngine;

namespace MustHave
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static GameObject GameObject => Instance.gameObject;
        public static Transform Transform => Instance.transform;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindAnyObjectByType<T>();

                    if (!instance)
                    {
                        Debug.LogError($"An instance of {typeof(T).Name} is needed in the scene, but there is none.");
                    }
                }
                return instance;
            }
        }

        protected static T instance = null;

        protected virtual void Awake()
        {
            if (instance && instance != this)
            {
                ObjectUtils.Destroy(gameObject);
            }
            else
            {
                instance = this as T;
                OnAwake();
            }
        }

        protected virtual void OnAwake() { }

        protected virtual void OnDestroy()
        {
            if (this == instance)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Create instance from prefab if it was not found in the scene
        /// </summary>
        /// <param name="prefab"></param>
        public static void FindOrCreateInstance(T prefab)
        {
            instance = (instance ?? FindAnyObjectByType<T>()) ?? Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
