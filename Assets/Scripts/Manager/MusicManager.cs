using System;
using QFramework;
using UnityEngine;

namespace ChaosBall.Manager
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoSingleton<MusicManager>
    {
        private AudioSource _bgmAudioSource;

        private void Awake()
        {
            _bgmAudioSource = GetComponent<AudioSource>();
        }

        public void StartBGM()
        {
            _bgmAudioSource.Play();
        }

        public void PauseBGM()
        {
            _bgmAudioSource.Pause();
        }

        public void StopBGM()
        {
            _bgmAudioSource.Stop();
        }
    }
}