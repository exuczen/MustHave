using UnityEngine;

namespace MustHave
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static GameObject GameObject => Instance.gameObject;
        public static Transform Transform => Instance.transform;

        protected static T instance = null;

        protected virtual void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this as T;
                OnAwake();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
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

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                           " is needed in the scene, but there is none.");
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Create instance from prefab if it was not found in the scene
        /// </summary>
        /// <param name="prefab"></param>
        public static void FindOrCreateInstance(T prefab)
        {
            instance = (instance ?? FindObjectOfType<T>()) ?? Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
