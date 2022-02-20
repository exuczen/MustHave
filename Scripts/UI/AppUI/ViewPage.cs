namespace MustHave.UI
{
    public class ViewPage : UIScript
    {
        public ViewPager ViewPager { get; set; } = default;

        public T CreateInstance<T>(ViewPager viewPager) where T : ViewPage
        {
            ViewPager = viewPager;
            T viewPage = Instantiate(this, this.ViewPager.content, false) as T;
            return viewPage;
        }
    }
}
