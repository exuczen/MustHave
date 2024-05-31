namespace MustHave
{
    interface ISelectionScrollViewPool
    {
        bool Show();
        void Hide();
        void CreateButtonsPool(int capacity = -1);
        void ReturnButtonsToPool();
    } 
}

