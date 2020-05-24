using System;
using UnityEngine;

namespace MustHave
{
    public class DataMessageEvent<T> : ScriptableObject, IMessageEvent
    {
        private event Action<T> _event = default;

        protected T _data = default;

        public T Data { get => _data; set => _data = value; }

        public void AddListener(Action<T> listener)
        {
            _event += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            _event -= listener;
        }

        public void Invoke(T data, bool setData = true)
        {
            _event?.Invoke(setData ? (_data = data) : data);
        }

        public void Invoke()
        {
            _event?.Invoke(_data);
        }

        public void RemoveAllListeners()
        {
            _event = null;
        }
    }
}
