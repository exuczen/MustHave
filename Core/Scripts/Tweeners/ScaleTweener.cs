using UnityEngine;

namespace MustHave.Tweeners
{
    public class ScaleTweener : Tweener
    {
        [SerializeField] private Vector3 deltaScale = default;
        [SerializeField] private bool absolute = false;

        protected override void OnUpdateRoutine(float intervalElapsedTime, float intervalDuration, float durationScaleFactor, float totalElapsedTime)
        {
            //Debug.Log(GetType() + ".OnUpdateRoutine: _scaleBounce: " + totalElapsedTime + " " + _totalDuration);
            TransitionType transitionType = absolute ? TransitionType.SIN_IN_PI_RANGE : TransitionType.SIN_IN_2PI_RANGE;
            float transition = Maths.GetTransition(transitionType, intervalElapsedTime, intervalDuration, 1);
            float amp = intervalIterations > 1 ? 1f - totalElapsedTime / totalDuration : 1f;
            transform.localScale = initialScale + amp * durationScaleFactor * transition * deltaScale;
        }
    }
}
