using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

namespace MustHave.Tweeners
{
    public class LissajouxTweener : MonoBehaviour
    {
        [SerializeField] private float _frequencyScale = 1f;
        [SerializeField] private float _totalPhaseDivPI = 0f;
        [SerializeField] private Vector3Int _frequency = Vector3Int.zero;
        [SerializeField] private Vector3 _amplitude = Vector3.zero;
        [SerializeField] private Vector3 _phaseDivPI = Vector3.zero;

        private Vector3 _initialPosition = default;
        private Coroutine _updateRoutine = default;
        private bool _isStarted = default;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            _initialPosition = transform.localPosition;
            _updateRoutine = StartCoroutine(UpdateRoutine());
            _isStarted = true;

        }

        private void OnEnable()
        {
            if (_isStarted && _updateRoutine == null)
                _updateRoutine = StartCoroutine(UpdateRoutine());
        }

        private void OnDisable()
        {
            if (_updateRoutine != null)
                StopCoroutine(_updateRoutine);
            _updateRoutine = null;
        }

        protected IEnumerator UpdateRoutine()
        {
            yield return CoroutineUtils.UpdateRoutine(() => true, elapsedTime => {
                Vector3 frequency = _frequency;
                frequency *= _frequencyScale;
                Vector3 phase = (_phaseDivPI + Vector3.one * _totalPhaseDivPI) * Mathf.PI;
                Vector3 translation = Mathv.Mul(_amplitude, Mathv.Sin(frequency * elapsedTime + phase));
                transform.localPosition = _initialPosition + translation;
            });
        }
    }
}
