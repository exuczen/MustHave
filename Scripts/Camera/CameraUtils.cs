﻿using UnityEngine;

namespace MustHave
{
    public struct CameraUtils
    {
        public static Camera MainOrCurrent => Camera.main ? Camera.main : Camera.current;
    }
}
