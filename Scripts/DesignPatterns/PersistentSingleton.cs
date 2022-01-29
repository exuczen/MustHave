using UnityEngine;

namespace MustHave
{
    public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void OnAwake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
