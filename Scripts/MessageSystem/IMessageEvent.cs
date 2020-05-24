namespace MustHave
{
    public interface IMessageEvent
    {
        void Invoke();
        void RemoveAllListeners();
    }
}
