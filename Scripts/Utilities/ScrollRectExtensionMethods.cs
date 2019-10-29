using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MustHave.Utilities
{
    public static class ScrollRectExtensionMethods
    {
        public static Coroutine SwipeToGrid(this ScrollRect scrollRect, HorizontalLayoutGroup layoutGroup, float minDuration, float maxDuration, Action onEnd)
        {
            int childCount = scrollRect.content.childCount;
            if (childCount > 1)
            {
                if (!layoutGroup.childControlWidth || !layoutGroup.childForceExpandWidth)
                {
                    throw new ArgumentException();
                }
                RectTransform viewport = scrollRect.viewport;
                RectTransform content = scrollRect.content;
                float decelerationRate = scrollRect.decelerationRate;
                float horizontalNormalizedPos = scrollRect.horizontalNormalizedPosition;
                float childNormalizedWidth = 1f / childCount;
                float contentNormalizedPos = horizontalNormalizedPos * (1f - childNormalizedWidth);
                int closestChildIndex = (int)((contentNormalizedPos + childNormalizedWidth / 2f) / childNormalizedWidth);
                closestChildIndex = Mathf.Clamp(closestChildIndex, 0, childCount - 1);
                float closestChildNormalizedPos = closestChildIndex * childNormalizedWidth / (1f - childNormalizedWidth);
                float velocity = scrollRect.velocity.x;
                float minVelocity = viewport.rect.width / 2f; //[1/s]
                float destNormalizedPos;
                float translation;
                float duration;
                if (Mathf.Abs(velocity) < minVelocity)
                {
                    destNormalizedPos = closestChildNormalizedPos;
                    translation = (horizontalNormalizedPos - destNormalizedPos) * (content.rect.width - viewport.rect.width);
                    velocity = Mathf.Sign(translation) * minVelocity;
                }
                else
                {
                    int destChildIndex;
                    if (horizontalNormalizedPos - closestChildNormalizedPos > 0f)
                    {
                        destChildIndex = velocity < 0f ? Mathf.Clamp(closestChildIndex + 1, 0, childCount - 1) : closestChildIndex;
                    }
                    else
                    {
                        destChildIndex = velocity < 0f ? closestChildIndex : Mathf.Clamp(closestChildIndex - 1, 0, childCount - 1);
                    }
                    destNormalizedPos = destChildIndex * childNormalizedWidth / (1f - childNormalizedWidth);
                    translation = (horizontalNormalizedPos - destNormalizedPos) * (content.rect.width - viewport.rect.width);
                }
                float deceleration = velocity * (decelerationRate - 1f); //[1/s]
                if (Mathf.Abs(deceleration) < 1f)
                    deceleration = Mathf.Sign(deceleration);
                float timeToStop = -velocity / deceleration;
                float translationToStop = timeToStop * (velocity + deceleration * timeToStop / 2f);
                if (Mathf.Abs(translationToStop) < Mathf.Abs(translation))
                {
                    duration = maxDuration;
                }
                else
                {
                    duration = (-velocity + Mathf.Sign(velocity) * Mathf.Sqrt(Mathf.Pow(velocity, 2f) + 2f * deceleration * translation)) / deceleration;
                    duration = Mathf.Clamp(duration, minDuration, maxDuration);
                }
                //Debug.Log("SwipeToGrid: " + horizontalNormalizedPos + "velocity=" + velocity + " minVelocity=" + minVelocity + " duration=" + duration);
                return scrollRect.StartCoroutine(scrollRect.SwipeRoutine(destNormalizedPos, duration, () => {
                    onEnd?.Invoke();
                }));
            }
            return null;
        }

        public static IEnumerator SwipeRoutine(this ScrollRect scrollRect, float destNormalizedPos, float duration, Action onEnd)
        {
            float initNormalizedPos = scrollRect.horizontalNormalizedPosition;
            yield return CoroutineUtils.UpdateRoutine(duration, (elapsedTime, transition) => {
                transition = Maths.GetTransition(TransitionType.PARABOLIC_DEC, transition, 2);
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(initNormalizedPos, destNormalizedPos, transition);
            });
            scrollRect.horizontalNormalizedPosition = destNormalizedPos;
            onEnd?.Invoke();
        }
    }
}
