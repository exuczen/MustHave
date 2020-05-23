using System;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(menuName = "MessageSystem/AppMessageEvents")]
    public class AppMessageEvents : MessageEventGroup
    {
        [SerializeField] private ShowScreenMessageEvent _showScreenMessage = default;
        [SerializeField] private MessageEvent _backToPrevScreenMessage = default;
        [SerializeField] private SetAlertPopupMessageEvent _setAlertPopupMessage = default;

        public ShowScreenMessageEvent ShowScreenMessage => _showScreenMessage;
        public MessageEvent BackToPrevScreenMessage => _backToPrevScreenMessage;
        public SetAlertPopupMessageEvent SetAlertPopupMessage => _setAlertPopupMessage;
    }
}
