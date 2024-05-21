using System.Collections;
using UnityEngine;

namespace MustHave.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SuicidalAudioScript : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
            Destroy(gameObject);
        }
    }
}
