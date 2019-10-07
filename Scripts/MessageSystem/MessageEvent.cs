using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu]
    public class MessageEvent : ScriptableObject
    {
        public object param;
    }

    public class DataMessageEvent<T> : MessageEvent
    {
        public T Data { get => (T)param; }
    }
}