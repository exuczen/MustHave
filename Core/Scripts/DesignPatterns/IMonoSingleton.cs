﻿using UnityEngine;

namespace MustHave
{
    public interface IMonoSingleton<T> where T : MonoBehaviour
    {
        public static GameObject GameObject => Instance.gameObject;
        public static Transform Transform => Instance.transform;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Object.FindAnyObjectByType<T>();

                    if (!instance)
                    {
                        Debug.LogError($"An instance of {typeof(T).Name} is needed in the scene, but there is none.");
                    }
                }
                return instance;
            }
        }

        protected static T instance = null;

        protected static void SetInstanceOnAwake(T mono, out GameObject gameObject)
        {
            gameObject = mono.gameObject;

            if (instance && instance != mono)
            {
                Debug.LogWarning($"An instance of {typeof(T).Name} is already in the scene. {mono.name} component is to be destroyed.");

                ObjectUtils.DestroyComponent(mono);
            }
            else
            {
                instance = mono;
            }
        }

        protected static void ClearInstanceOnDestroy(T mono)
        {
            if (mono == instance)
            {
                instance = null;
            }
        }
    }
}
