using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MustHave.Audio
{
    [CreateAssetMenu]
    public class CustomAudioClip : ScriptableObject
    {
        [SerializeField]
        private AudioClip _audioClip = default;
        [SerializeField, Range(0f, 1f)]
        private float _volume = 1f;

        public AudioClip AudioClip { get => _audioClip; }
        public float Volume { get => _volume; }
    } 
}

