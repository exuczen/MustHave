using UnityEngine;

namespace MustHave
{
    public struct CameraUtils
    {
        public static Camera MainOrCurrent { get => Camera.main ?? Camera.current; }
    }
}
