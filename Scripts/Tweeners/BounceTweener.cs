using UnityEngine;

namespace MustHave.Tweeners
{
    public class BounceTweener : Tweener
    {
        [SerializeField] private Vector3 translation = default;

        protected override void OnUpdateRoutine(float intervalElapsedTime, float intervalDuration, float durationScaleFactor, float totalElapsedTime)
        {
            float transition = Maths.GetTransition(TransitionType.PARABOLIC_DEC, intervalElapsedTime, intervalDuration / 2f, 2);
            transform.localPosition = initialPosition + durationScaleFactor * transition * translation;
        }
    }
}
