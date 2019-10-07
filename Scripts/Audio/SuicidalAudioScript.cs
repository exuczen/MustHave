using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SuicidalAudioScript : MonoBehaviour
    {
        private IEnumerator Start()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
            Destroy(gameObject);
        }
    }
}
