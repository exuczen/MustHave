using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MustHave.Audio
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlesAudioScript : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _suicidalAudioPrefab = default;
        [SerializeField]
        private AudioClip _birthClip = default;
        [SerializeField]
        private AudioClip _deathClip = default;
        [SerializeField, Range(0f, 1f)]
        private float _birthClipVolume = 1f;
        [SerializeField, Range(0f, 1f)]
        private float _deathClipVolume = 1f;

        private ParticleSystem _particleSystem = default;
        private int _particlesCount = default;
        private bool _running = default;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            _running = true;
        }

        private void Update()
        {
            if (_running)
            {
                if (_birthClip && _particleSystem.particleCount > _particlesCount)
                {
                    CreateAudioSource(_birthClip, _birthClipVolume).Play();
                }
                else if (_deathClip && _particleSystem.particleCount < _particlesCount)
                {
                    CreateAudioSource(_deathClip, _deathClipVolume).Play();
                }
                _particlesCount = _particleSystem.particleCount;
            }
        }

        private AudioSource CreateAudioSource(AudioClip audioClip, float volume)
        {
            //Debug.Log(GetType() + ".CreateAudioSource: " + audioClip);
            AudioSource audioSource = AudioSource.Instantiate(_suicidalAudioPrefab, transform, false);
            audioSource.transform.localPosition = Vector2.zero;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            return audioSource;
        }

        private void DestroyAllAudios()
        {
            SuicidalAudioScript[] audios = GetComponentsInChildren<SuicidalAudioScript>(true);
            for (int i = 0; i < audios.Length; i++)
            {
                Destroy(audios[i].gameObject);
            }
        }
    } 
}
