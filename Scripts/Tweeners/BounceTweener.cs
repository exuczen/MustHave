using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

namespace MustHave.Tweeners
{
    public class BounceTweener : TweenerScript
    {
        [SerializeField] private Vector3 _translation = default;

        protected override void OnUpdateRoutine(float intervalElapsedTime, float intervalDuration, float durationScaleFactor, float totalElapsedTime)
        {
            float transition = Maths.GetTransition(TransitionType.PARABOLIC_DEC, intervalElapsedTime, intervalDuration / 2f, 2);
            transform.localPosition = _initialPosition + durationScaleFactor * transition * _translation;
        }
    }
}
