using UnityEngine;

namespace MustHave.Audio
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlesAudioScript : MonoBehaviour
    {
        [SerializeField] private AudioSource suicidalAudioPrefab = default;
        [SerializeField] private AudioClip birthClip = default;
        [SerializeField] private AudioClip deathClip = default;
        [SerializeField, Range(0f, 1f)] private float birthClipVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float deathClipVolume = 1f;

        private new ParticleSystem particleSystem = default;
        private int particlesCount = 0;
        private bool running = false;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            running = true;
        }

        private void Update()
        {
            if (running)
            {
                if (birthClip && particleSystem.particleCount > particlesCount)
                {
                    CreateAudioSource(birthClip, birthClipVolume).Play();
                }
                else if (deathClip && particleSystem.particleCount < particlesCount)
                {
                    CreateAudioSource(deathClip, deathClipVolume).Play();
                }
                particlesCount = particleSystem.particleCount;
            }
        }

        private AudioSource CreateAudioSource(AudioClip audioClip, float volume)
        {
            //Debug.Log(GetType() + ".CreateAudioSource: " + audioClip);
            var audioSource = Instantiate(suicidalAudioPrefab, transform, false);
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
