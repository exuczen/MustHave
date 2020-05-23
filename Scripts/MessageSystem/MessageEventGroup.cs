using System;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(menuName = "MessageSystem/MessageEventGroup")]
    public class MessageEventGroup : ScriptableObject
    {
        [SerializeField] private List<MessageEvent> _events = default;

        public void ClearEventListeners()
        {
            foreach (var e in _events)
            {
                e.Clear();
            }
        }
    }
}
