using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(menuName = "MessageSystem/MessageEventGroup")]
    public class MessageEventGroup : ScriptableObject
    {
        [SerializeField] private List<MessageEvent> _events = new List<MessageEvent>();

        public void RemoveAllListeners()
        {
            _events.ForEach(e => e.RemoveAllListeners());
        }
    }
}
