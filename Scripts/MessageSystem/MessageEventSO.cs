using UnityEngine;

namespace MustHave
{
    public abstract class MessageEventSO : ScriptableObject
    {
        public abstract void Invoke();
        public abstract void RemoveAllListeners();
    }
}
