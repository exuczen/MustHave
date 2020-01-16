using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;
using MustHave.UI;

namespace MustHave.Tweeners
{
    public abstract class TweenerScript : MonoBehaviour
    {
        [SerializeField] private bool _randomDelay = default;
        [SerializeField, ConditionalHide("_randomDelay", true)] private float _randomDelayMin = default;
        [SerializeField, ConditionalHide("_randomDelay", true)] private float _randomDelayMax = default;
        [SerializeField] private float _intervalDuration = 1f;
        [SerializeField] protected int _intervalIterations = 1;
        [SerializeField] protected bool _loop = true;
        [SerializeField] protected AudioClip _audioClip = default;

        protected float _totalDuration = default;
        protected Vector3 _initialPosition = default;
        protected Vector3 _initialScale = default;
        private Coroutine _updateRoutine = default;
        private bool _isStarted = default;
        private AudioSource _audioSource = default;

        private void Awake()
        {
            List<AudioSource> audioSourceList = new List<AudioSource>();
            GetComponents<AudioSource>(audioSourceList);
            _audioSource = audioSourceList.Find(source => source.clip == null);
            if (_audioSource)
                _audioSource.clip = _audioClip;
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            _intervalIterations = Mathf.Max(_intervalIterations, 1);
            _totalDuration = Maths.GetGeometricSum(_intervalDuration, 0.5f, _intervalIterations);
            _initialPosition = transform.localPosition;
            _initialScale = transform.localScale;
            _isStarted = true;
            _updateRoutine = StartUpdate(_intervalDuration);
        }

        private void OnEnable()
        {
            if (_isStarted && _updateRoutine == null)
                _updateRoutine = StartUpdate(_intervalDuration);
        }

        private void OnDisable()
        {
            if (_updateRoutine != null)
                StopCoroutine(_updateRoutine);
            _updateRoutine = null;
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
            float totalElapsedTime = 0f;
            while (true)
            {
                if (_audioSource)
                {
                    _audioSource.volume = durationScaleFactor;
                    _audioSource.Play();
                }
                while (_intervalDuration < 0 || (intervalElapsedTime = Time.time - intervalStartTime) < intervalDuration)
                {
                    totalElapsedTime = Mathf.Min(_totalDuration, Time.time - startTime);
                    OnUpdateRoutine(intervalElapsedTime, intervalDuration, durationScaleFactor, totalElapsedTime);
                    yield return null;
                }
                intervalStartTime += (int)(intervalElapsedTime / intervalDuration) * intervalDuration;
                intervalCounter++;
                if (intervalCounter >= _intervalIterations)
                {
                    if (_randomDelay)
                    {
                        intervalElapsedTime = intervalDuration;
                        totalElapsedTime = Mathf.Min(_totalDuration, Time.time - startTime);
                        OnUpdateRoutine(intervalElapsedTime, intervalDuration, durationScaleFactor, _totalDuration);
                        yield return new WaitForSeconds(Random.Range(_randomDelayMin, _randomDelayMax));
                        intervalStartTime = Time.time;
                    }
                    startTime = Time.time;
                    durationScaleFactor = 1f;
                    intervalDuration = duration;
                    intervalCounter = 0;
                    if (!_loop)
                    {
                        OnUpdateRoutine(0f, intervalDuration, durationScaleFactor, _totalDuration);
                        yield return new WaitForEndOfFrame();
                        this.enabled = false;
                        yield break;
                    }
                }
                else if (_intervalIterations > 1)
                {
                    durationScaleFactor = 1f / (intervalCounter << 1);
                    intervalDuration = duration * durationScaleFactor;
                }
            }
        }

        private void PlayAudioClip()
        {
            _audioSource?.Play();
        }

        public Coroutine StartUpdate(float duration)
        {
            return StartCoroutine(UpdateRoutine(duration));
        }
    }
}
