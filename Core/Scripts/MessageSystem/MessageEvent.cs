namespace MustHave
{
    public abstract class MessageEvent
    {
        public abstract void Invoke();
        public abstract void RemoveAllListeners();
    }
}
