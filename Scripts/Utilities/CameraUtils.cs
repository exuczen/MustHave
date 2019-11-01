using UnityEngine;

public class CameraUtils
{
    public static Camera MainOrCurrent { get => Camera.main ?? Camera.current; }
}
