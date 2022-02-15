using MustHave.UI;

namespace MustHave
{
    public class AppMessageEvents : MessageEventGroup
    {
        public DataMessageEvent<ScreenData> ShowScreenMessage { get; } = new DataMessageEvent<ScreenData>();
        public ActionMessageEvent BackToPrevScreenMessage { get; } = new ActionMessageEvent();
        public TypeMessageEvent SetAlertPopupMessage { get; } = new TypeMessageEvent();

        public override void Initialize()
        {
            AddToList(ShowScreenMessage, BackToPrevScreenMessage, SetAlertPopupMessage);
        }
    }
}
