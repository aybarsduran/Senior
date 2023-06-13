using UnityEngine;

namespace IdenticalStudios.Surfaces
{
    public sealed class SurfaceEffect : MonoBehaviour
    {
        [SerializeField]
        private SoundPlayer m_Audio;

        [SerializeField, HideInInspector]
        private ParticleSystem[] m_Particles;

        [SerializeField, HideInInspector]
        private AudioSource m_AudioSource;


        public void Init(SoundPlayer audioEffect, GameObject visualEffect, bool spatializeAudio)
        {
            m_Audio = audioEffect;

            m_AudioSource = AudioManager.CreateAudioSource(gameObject, false, 1f, 1f, AudioManager.MixerOutputGroup.Effects);
            m_AudioSource.spatialBlend = 1f;

            m_AudioSource.spatialize = spatializeAudio;

            if (visualEffect != null)
            {
                Instantiate(visualEffect, transform.position, transform.rotation, transform);
                m_Particles = GetComponentsInChildren<ParticleSystem>();
            }
        }

        public void Play(float audioVolume)
        {
            m_Audio.Play(m_AudioSource, audioVolume);

            for (int i = 0;i < m_Particles.Length;i++)
                m_Particles[i].Play(false);
        }
    }
}