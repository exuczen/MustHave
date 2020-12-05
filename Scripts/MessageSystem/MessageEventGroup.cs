using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    public abstract class MessageEventGroup : ScriptableObject
    {
        protected List<MessageEvent> _events = new List<MessageEvent>();

        public abstract void Initialize();

        public void RemoveAllListeners()
        {
            _events.ForEach(e => e.RemoveAllListeners());
        }

        protected void AddToList(params MessageEvent[] messageEvents)
        {
            foreach (var messageEvent in messageEvents)
            {
                if (messageEvent != null && !_events.Contains(messageEvent))
                {
                    _events.Add(messageEvent);
                }
            }
        }
    }
}
