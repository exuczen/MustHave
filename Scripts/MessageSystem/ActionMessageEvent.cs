using System;
using UnityEngine;

namespace MustHave
{
    public class ActionMessageEvent : MessageEvent
    {
        private event Action _event = default;

        public void AddListener(Action listener)
        {
            _event += listener;
        }

        public void RemoveListener(Action listener)
        {
            _event -= listener;
        }

        public override void Invoke()
        {
            _event?.Invoke();
        }

        public override void RemoveAllListeners()
        {
            _event = null;
        }
    }
}
