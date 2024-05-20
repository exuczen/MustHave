﻿using System;
using System.Collections;
using UnityEngine;

namespace MustHave.Utils
{
    public readonly struct CoroutineUtils
    {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new();

        public static IEnumerator WaitForFrames(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return null;
            }
        }

        public static IEnumerator ActionAfterEndOfFrame(Action action)
        {
            yield return WaitForEndOfFrame;
            action?.Invoke();
        }

        public static IEnumerator ActionAfterTimeEndOfFrame(Action action, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            yield return WaitForEndOfFrame;
            action?.Invoke();
        }

        public static IEnumerator ActionAfterPredicate(Action action, Func<bool> predicate)
        {
            yield return new WaitWhile(predicate);
            action?.Invoke();
        }

        public static IEnumerator ActionAfterCustomYieldInstruction(Action action, CustomYieldInstruction yieldInstruction)
        {
            yield return yieldInstruction;
            action?.Invoke();
        }

        public static IEnumerator ActionAfterTime(Action action, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            action?.Invoke();
        }

        public static IEnumerator ActionAfterRealtime(Action action, float delayInSeconds)
        {
            yield return new WaitForSecondsRealtime(delayInSeconds);
            action?.Invoke();
        }

        public static IEnumerator ActionAfterFrames(Action action, int framesCount)
        {
            yield return WaitForFrames(framesCount);
            action?.Invoke();
        }

        public static IEnumerator FixedUpdateRoutine(float duration, Action<float, float> onUpdate)
        {
            yield return UpdateRoutine(duration, onUpdate, new WaitForFixedUpdate());
        }

        public static IEnumerator FixedUpdateRoutine(Func<bool> predicate, Action<float> onUpdate)
        {
            yield return UpdateRoutine(predicate, onUpdate, new WaitForFixedUpdate());
        }

        public static IEnumerator UpdateRoutine(Func<bool> predicate, Action<float> onUpdate, YieldInstruction yieldInstruction = null)
        {
            float startTime = Time.time;
            while (predicate())
            {
                onUpdate.Invoke(Time.time - startTime);
                yield return yieldInstruction;
            }
        }

        public static IEnumerator UpdateRoutine(float duration, Action<float, float> onUpdate, YieldInstruction yieldInstruction = null, bool finalize = false)
        {
            float startTime = Time.time;
            float elapsedTime;
            while ((elapsedTime = Time.time - startTime) < duration)
            {
                onUpdate.Invoke(elapsedTime, elapsedTime / duration);
                yield return yieldInstruction;
            }
            if (finalize)
            {
                onUpdate.Invoke(duration, 1f);
            }
        }

        public static IEnumerator UpdateRoutine(float duration, IEnumerator onStartRoutine, Action<float, float> onUpdate, IEnumerator onEndRoutine)
        {
            if (onStartRoutine != null)
            {
                yield return onStartRoutine;
            }
            yield return UpdateRoutine(duration, onUpdate);
            if (onEndRoutine != null)
            {
                yield return onEndRoutine;
            }
        }

        public static IEnumerator UpdateRoutine(float duration, Action onStart, Action<float, float> onUpdate, Action onEnd)
        {
            onStart?.Invoke();
            yield return UpdateRoutine(duration, onUpdate);
            onEnd?.Invoke();
        }
    }
}