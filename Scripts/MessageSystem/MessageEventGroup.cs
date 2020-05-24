using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(menuName = "MessageSystem/MessageEventGroup")]
    public class MessageEventGroup : ScriptableObject
    {
        [SerializeField] private List<MessageEventSO> _events = new List<MessageEventSO>();

        public void RemoveAllListeners()
        {
            _events.ForEach(e => e.RemoveAllListeners());
        }
    }
}
