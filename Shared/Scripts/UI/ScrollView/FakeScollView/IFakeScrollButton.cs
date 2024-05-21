using UnityEngine;

namespace MustHave.UI
{
    public interface IFakeScrollButton
    {
        Transform FakeScrollButtonTransform { get; }
        void ClearFakeScrollButtonContent();
        void SetupFakeScrollButtonContent(object content);
    }
}
