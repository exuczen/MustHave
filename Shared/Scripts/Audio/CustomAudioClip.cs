using UnityEngine;

namespace MustHave.Audio
{
    [CreateAssetMenu(fileName = "CustomAudioClip", menuName = "MustHave/ScriptableObjects/CustomAudioClip")]
    public class CustomAudioClip : ScriptableObject
    {
        [SerializeField] private AudioClip audioClip = default;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        public AudioClip AudioClip => audioClip;
        public float Volume => volume;
    }
}

