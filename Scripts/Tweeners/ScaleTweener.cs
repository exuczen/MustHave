using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

namespace MustHave.Tweeners
{
    public class ScaleTweener : TweenerScript
    {
        [SerializeField]
        private Vector3 _deltaScale = default;
        [SerializeField]
        private bool _absolute = false;

        protected override void OnUpdateRoutine(float intervalElapsedTime, float intervalDuration, float durationScaleFactor, float totalElapsedTime)
        {
            //Debug.Log(GetType() + ".OnUpdateRoutine: _scaleBounce: " + totalElapsedTime + " " + _totalDuration);
            TransitionType transitionType = _absolute ? TransitionType.SIN_IN_PI_RANGE : TransitionType.SIN_IN_2PI_RANGE;
            float transition = Maths.GetTransition(transitionType, intervalElapsedTime, intervalDuration, 1);
            float amp = _intervalIterations > 1 ? 1f - totalElapsedTime / _totalDuration : 1f;
            transform.localScale = _initialScale + amp * durationScaleFactor * transition * _deltaScale;
        }
    } 
}
