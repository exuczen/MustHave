using System.Collections;
using UnityEngine;

namespace MustHave.Tweeners
{
    public class LissajouxTweener : MonoBehaviour
    {
        [SerializeField] private float frequencyScale = 1f;
        [SerializeField] private float totalPhaseDivPI = 0f;
        [SerializeField] private Vector3Int frequency = Vector3Int.zero;
        [SerializeField] private Vector3 amplitude = Vector3.zero;
        [SerializeField] private Vector3 phaseDivPI = Vector3.zero;

        private Vector3 initialPosition = default;
        private Coroutine updateRoutine = default;
        private bool isStarted = default;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            initialPosition = transform.localPosition;
            updateRoutine = StartCoroutine(UpdateRoutine());
            isStarted = true;

        }

        private void OnEnable()
        {
            if (isStarted && updateRoutine == null)
            {
                updateRoutine = StartCoroutine(UpdateRoutine());
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

        protected IEnumerator UpdateRoutine()
        {
            yield return CoroutineUtils.UpdateRoutine(() => true, elapsedTime => {
                Vector3 scaledFrequency = frequency;
                scaledFrequency *= frequencyScale;
                Vector3 phase = (phaseDivPI + Vector3.one * totalPhaseDivPI) * Mathf.PI;
                Vector3 translation = Mathv.Mul(amplitude, Mathv.Sin(scaledFrequency * elapsedTime + phase));
                transform.localPosition = initialPosition + translation;
            });
        }
    }
}
