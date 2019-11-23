using System;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(menuName = "MessageSystem/MessageBus")]
    public class MessageBus : ScriptableObject
    {
        private List<Action<MessageEvent>> _listeners = default;

        public List<Action<MessageEvent>> Listeners { get => _listeners; }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            Clear();
        }

        public void Register(Action<MessageEvent> callback)
        {
            _listeners.Add(callback);
        }

        public void Unregister(Action<MessageEvent> callback)
        {
            _listeners.Remove(callback);
        }

        public void Notify(MessageEvent message)
        {
            foreach (var l in _listeners)
            {
                l.Invoke(message);
            }
        }

        public void Init()
        {
            _listeners = new List<Action<MessageEvent>>();
        }

        public void Clear()
        {
            _listeners.Clear();
        }

        public void PrintListeners()
        {
            Debug.Log(GetType() + ".PrintListeners: ");
            foreach (var listener in _listeners)
            {
                Debug.Log(GetType() + ".Listener: " + listener.Target.GetType() + "." + listener.Method.Name);
            }
        }
    }
}
