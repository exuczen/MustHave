using System;

namespace MustHave
{
    public class ActionMessageEvent : MessageEvent
    {
        private event Action Event = default;

        public void AddListener(Action listener)
        {
            Event += listener;
        }

        public void RemoveListener(Action listener)
        {
            Event -= listener;
        }

        public override void Invoke()
        {
            Event?.Invoke();
        }

        public override void RemoveAllListeners()
        {
            Event = null;
        }
    }
}
