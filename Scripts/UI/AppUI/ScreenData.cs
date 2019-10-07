
using System;
using UnityEngine;

namespace MustHave.UI
{
    public class ScreenData
    {
        private ScreenScript _screen = default;
        private Type _screenType = default;
        private Type _canvasType = default;
        private string _sceneName = default;
        private bool _keepOnStack = true;
        private bool _clearStack = true;

        public ScreenScript Screen { get => _screen; set => _screen = value; }
        public Type ScreenType { get => _screenType; }
        public Type CanvasType { get => _canvasType; }
        public string SceneName { get => _sceneName; }
        public bool KeepOnStack { get => _keepOnStack; set => _keepOnStack = value; }
        public bool ClearStack { get => _clearStack; set => _clearStack = value; }

        public ScreenData(Type screenType, Type canvasType, string sceneName, bool keepOnStack = true, bool clearStack = false)
        {
            _screen = null;
            _screenType = screenType;
            _canvasType = canvasType;
            _sceneName = sceneName;
            _keepOnStack = keepOnStack;
            _clearStack = clearStack;
        }

        public ScreenData(ScreenScript screen, bool keepOnStack = true, bool clearStack = false)
        {
            //Debug.Log(GetType() + ".ScreenData" + screen + " " + screen.Canvas);
            _screen = screen;
            _screenType = screen.GetType();
            _canvasType = screen.Canvas.GetType();
            _sceneName = screen.Canvas.SceneName;
            _keepOnStack = keepOnStack;
            _clearStack = clearStack;
        }
    }
}
