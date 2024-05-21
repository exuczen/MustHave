using System;

namespace MustHave
{
    public class DataMessageEvent<T> : MessageEvent
    {
        private event Action<T> Event = default;

        protected T data = default;

        public T Data { get => data; set => data = value; }

        public void AddListener(Action<T> listener)
        {
            Event += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            Event -= listener;
        }

        public void Invoke(T data, bool setData = true)
        {
            Event?.Invoke(setData ? (this.data = data) : data);
        }

        public override void Invoke()
        {
            Event?.Invoke(data);
        }

        public override void RemoveAllListeners()
        {
            Event = null;
        }
    }
}
