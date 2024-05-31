﻿using System;
using System.Collections;
using UnityEngine;

namespace MustHave.Utils
{
    public static class MonoBehaviourExtensionMethods
    {
        public static Coroutine StartCoroutineActionAfterEndOfFrame(this MonoBehaviour mono, Action action)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterEndOfFrame(action));
        }

        public static Coroutine StartCoroutineActionAfterTimeEndOfFrame(this MonoBehaviour mono, Action action, float delayInSeconds)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterTimeEndOfFrame(action, delayInSeconds));
        }

        public static Coroutine StartCoroutineActionAfterTime(this MonoBehaviour mono, Action action, float delayInSeconds)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterTime(action, delayInSeconds));
        }

        public static Coroutine StartCoroutineActionAfterRealtime(this MonoBehaviour mono, Action action, float delayInSeconds)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterRealtime(action, delayInSeconds));
        }

        public static Coroutine StartCoroutineActionAfterFrames(this MonoBehaviour mono, Action action, int framesNumber)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterFrames(action, framesNumber));
        }

        public static Coroutine StartCoroutineActionAfterPredicate(this MonoBehaviour mono, Action action, Func<bool> predicate)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterPredicate(action, predicate));
        }

        public static Coroutine StartCoroutineActionAfterCustomYieldInstruction(this MonoBehaviour mono, Action action, CustomYieldInstruction yieldInstruction)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterCustomYieldInstruction(action, yieldInstruction));
        }

        public static Coroutine StartUpdateCoroutine(this MonoBehaviour mono, float duration, Action<float, float> onUpdate)
        {
            return mono.StartCoroutine(CoroutineUtils.UpdateRoutine(duration, onUpdate));
        }

        public static Coroutine StartUpdateCoroutine(this MonoBehaviour mono, float duration, IEnumerator onStartRoutine, Action<float, float> onUpdate, IEnumerator onEndRoutine)
        {
            return mono.StartCoroutine(CoroutineUtils.UpdateRoutine(duration, onStartRoutine, onUpdate, onEndRoutine));
        }

        public static Coroutine StartUpdateCoroutine(this MonoBehaviour mono, float duration, Action onStart, Action<float, float> onUpdate, Action onEnd)
        {
            return mono.StartCoroutine(CoroutineUtils.UpdateRoutine(duration, onStart, onUpdate, onEnd));
        }

        public static void StartCoroutine(this MonoBehaviour mono, IEnumerator enumerator, ref Coroutine coroutine)
        {
            mono.StopCoroutine(ref coroutine);
            coroutine = mono.StartCoroutine(enumerator);
        }

        public static void StopCoroutine(this MonoBehaviour mono, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
