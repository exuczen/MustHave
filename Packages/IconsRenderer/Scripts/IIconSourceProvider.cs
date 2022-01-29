namespace MustHave.IconsRenderer
{
    public interface IIconSourceProvider
    {
        int IconSourceCount { get; }
        IconSourceObject GetIconSourcePrefab(int index);
    }
}
