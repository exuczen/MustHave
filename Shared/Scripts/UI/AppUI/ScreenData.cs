using System;

namespace MustHave.UI
{
    public class ScreenData
    {
        public UIScreen Screen { get; set; } = default;
        public Type ScreenType { get; } = default;
        public Type CanvasType { get; } = default;
        public string SceneName { get; } = default;
        public bool KeepOnStack { get; set; } = true;
        public bool ClearStack { get; set; } = true;

        public ScreenData(Type screenType, Type canvasType, string sceneName, bool keepOnStack = true, bool clearStack = false)
        {
            Screen = null;
            ScreenType = screenType;
            CanvasType = canvasType;
            SceneName = sceneName;
            KeepOnStack = keepOnStack;
            ClearStack = clearStack;
        }

        public ScreenData(UIScreen screen, bool keepOnStack = true, bool clearStack = false)
        {
            //Debug.Log(GetType() + ".ScreenData" + screen + " " + screen.Canvas);
            Screen = screen;
            ScreenType = screen.GetType();
            CanvasType = screen.Canvas.GetType();
            SceneName = screen.Canvas.SceneName;
            KeepOnStack = keepOnStack;
            ClearStack = clearStack;
        }
    }
}
