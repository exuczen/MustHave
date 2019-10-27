using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu]
    public class MessageEvent : ScriptableObject { }

    public class DataMessageEvent<T> : MessageEvent
    {
        protected T _data = default;

        public T Data { get => _data; set => _data = value; }
    }
}