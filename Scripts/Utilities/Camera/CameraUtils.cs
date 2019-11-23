using UnityEngine;

namespace MustHave.Utilities
{
    public struct CameraUtils
    {
        public static Camera MainOrCurrent { get => Camera.main ?? Camera.current; }
    }
}
