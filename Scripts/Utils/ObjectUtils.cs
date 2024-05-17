using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MustHave.Utils
{
    public struct ObjectUtils
    {
        public static void DestroyComponent<T>(ref T component) where T : Component
        {
            DestroyComponentOrGameObject(ref component, c => c);
        }

        public static void DestroyGameObject<T>(ref T component) where T : Component
        {
            DestroyComponentOrGameObject(ref component, c => c.gameObject);
        }

        public static void DestroyComponentOrGameObject<T>(ref T component, Func<T, Object> getObject) where T : Component
        {
            if (component && component.gameObject)
            {
                Destroy(getObject(component));
            }
            component = null;
        }

        public static void Destroy(Object obj)
        {
            if (obj)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(obj);
                }
                else if (SceneUtils.IsActiveSceneLoadedAndValid())
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }
    }
}
