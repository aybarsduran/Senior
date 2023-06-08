using System;
using UnityEngine;

namespace IdenticalStudios
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioEffect : MonoBehaviour
    {
        #region Internal
        [Serializable]
        private class NoiseSettings
        {
            public bool Enabled;

            [Range(0f, 1f)]
            public float Intensity = 0.05f;

            [Range(0f, 10f)]
            public float Speed = 1f;
        }
        #endregion

        [SerializeField]
        private bool m_PlayOnAwake;

        [SerializeField, Range(0, 1f)]
        private float m_Volume;

        [SerializeField, Range(0f, 2f)]
        private float m_FadeDuration = 0.5f;

        [SerializeField]
        private NoiseSettings m_Noise;

        private AudioSource m_AudioSource;

        private bool m_IsPlaying;
        private float m_Weight;


        public void Play() => m_IsPlaying = true;
        public void Stop() => m_IsPlaying = false;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();

            if (m_PlayOnAwake)
            {
                Play();
                m_AudioSource.Play();
            }
        }

        private void FixedUpdate()
        {
            m_Weight = Mathf.MoveTowards(m_Weight, m_IsPlaying ? 1f : 0f, Time.deltaTime * (1f / m_FadeDuration));

            float noise = Mathf.PerlinNoise(Time.time * m_Noise.Speed, 0f) * m_Noise.Intensity;

            m_AudioSource.volume = (m_Volume + noise * m_Volume) * m_Weight;
        }
    }
}