using System;
using MustHave.UI;

namespace MustHave
{
    public class AppMessageEvents : MessageEventGroup
    {
        public DataMessageEvent<ScreenData> ShowScreenMessage { get; } = new DataMessageEvent<ScreenData>();
        public ActionMessageEvent BackToPrevScreenMessage { get; } = new ActionMessageEvent();
        public DataMessageEvent<Type> SetAlertPopupMessage { get; } = new DataMessageEvent<Type>();

        public override void Initialize()
        {
            AddToList(ShowScreenMessage, BackToPrevScreenMessage, SetAlertPopupMessage);
        }
    }
}
