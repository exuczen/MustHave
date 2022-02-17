using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utils;
using MustHave.UI;

namespace MustHave.Tweeners
{
    public abstract class Tweener : MonoBehaviour
    {
        [SerializeField] private bool randomDelay = default;
        [SerializeField, ConditionalHide("randomDelay", true)] private float randomDelayMin = default;
        [SerializeField, ConditionalHide("randomDelay", true)] private float randomDelayMax = default;
        [SerializeField] private float intervalDuration = 1f;
        [SerializeField] protected int intervalIterations = 1;
        [SerializeField] protected bool loop = true;
        [SerializeField] protected AudioClip audioClip = default;

        protected float totalDuration = default;
        protected Vector3 initialPosition = default;
        protected Vector3 initialScale = default;
        private Coroutine updateRoutine = default;
        private bool isStarted = default;
        private AudioSource audioSource = default;

        private void Awake()
        {
            List<AudioSource> audioSourceList = new List<AudioSource>();
            GetComponents<AudioSource>(audioSourceList);
            audioSource = audioSourceList.Find(source => source.clip == null);
            if (audioSource)
            {
                audioSource.clip = audioClip;
            }
        }

        private IEnumerator Start()
        {
            yield return CoroutineUtils.WaitForEndOfFrame;
            intervalIterations = Mathf.Max(intervalIterations, 1);
            totalDuration = Maths.GetGeometricSum(intervalDuration, 0.5f, intervalIterations);
            initialPosition = transform.localPosition;
            initialScale = transform.localScale;
            isStarted = true;
            updateRoutine = StartUpdate(intervalDuration);
        }

        private void OnEnable()
        {
            if (isStarted && updateRoutine == null)
            {
                updateRoutine = StartUpdate(intervalDuration);
            }
        }

        private void OnDisable()
        {
            if (updateRoutine != null)
            {
                StopCoroutine(updateRoutine);
            }
            updateRoutine = null;
        }


        protected virtual void OnUpdateRoutine(float intervalElapsedTime, float intervalDuration, float durationScaleFactor, float totalElapsedTime) { }

        protected IEnumerator UpdateRoutine(float duration)
        {
            float startTime = Time.time;
            float intervalStartTime = Time.time;
            float intervalElapsedTime = 0f;
            int intervalCounter = 0;
            float intervalDuration = duration;
            float durationScaleFactor = 1f;
            float totalElapsedTime;
            while (true)
            {
                if (audioSource)
                {
                    audioSource.volume = durationScaleFactor;
                    audioSource.Play();
                }
                while (this.intervalDuration < 0 || (intervalElapsedTime = Time.time - intervalStartTime) < intervalDuration)
                {
                    totalElapsedTime = Mathf.Min(totalDuration, Time.time - startTime);
                    OnUpdateRoutine(intervalElapsedTime, intervalDuration, durationScaleFactor, totalElapsedTime);
                    yield return null;
                }
                intervalStartTime += (int)(intervalElapsedTime / intervalDuration) * intervalDuration;
                intervalCounter++;
                if (intervalCounter >= intervalIterations)
                {
                    if (randomDelay)
                    {
                        intervalElapsedTime = intervalDuration;
                        //totalElapsedTime = Mathf.Min(totalDuration, Time.time - startTime);
                        OnUpdateRoutine(intervalElapsedTime, intervalDuration, durationScaleFactor, totalDuration);
                        yield return new WaitForSeconds(Random.Range(randomDelayMin, randomDelayMax));
                        intervalStartTime = Time.time;
                    }
                    startTime = Time.time;
                    durationScaleFactor = 1f;
                    intervalDuration = duration;
                    intervalCounter = 0;
                    if (!loop)
                    {
                        OnUpdateRoutine(0f, intervalDuration, durationScaleFactor, totalDuration);
                        yield return CoroutineUtils.WaitForEndOfFrame;
                        this.enabled = false;
                        yield break;
                    }
                }
                else if (intervalIterations > 1)
                {
                    durationScaleFactor = 1f / (intervalCounter << 1);
                    intervalDuration = duration * durationScaleFactor;
                }
            }
        }

        public Coroutine StartUpdate(float duration)
        {
            return StartCoroutine(UpdateRoutine(duration));
        }
    }
}
