using UnityEngine;

namespace MustHave
{
    public abstract class MessageEvent : ScriptableObject
    {
        public abstract void Invoke();
        public abstract void RemoveAllListeners();
    }
}
