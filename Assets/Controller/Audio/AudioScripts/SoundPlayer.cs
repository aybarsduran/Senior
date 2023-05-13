using System;
using System.Linq;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;
using UnityEngine.UIElements;

namespace IdenticalStudios
{
    [Serializable]
    public class SoundPlayer
    {
        public float Volume => m_Volume;
        public float Pitch => m_Pitch;
        public AudioClip[] Clips => m_AudioClips;

        [SerializeField, Range(0f, 2f)]
        protected float m_Volume = 0.65f;

        [SerializeField, Range(0f, 0.5f)]
        protected float m_VolumeJitter = 0.1f;

        [SerializeField, Range(0f, 2f)]
        protected float m_Pitch = 1f;

        [SerializeField, Range(0f, 0.5f)]
        protected float m_PitchJitter = 0.1f;

        protected AudioClip[] m_AudioClips;

        protected int m_LastClipPlayed = -1;


        public void Play(AudioSource audioSource, float volumeFactor = 1f, SelectionType selectionMethod = SelectionType.RandomExcludeLast)
        {
            if (!audioSource || !CanPlay())
                return;

            if (m_LastClipPlayed >= m_AudioClips.Length)
                m_LastClipPlayed = m_AudioClips.Length - 1;

            AudioClip clipToPlay = m_AudioClips.Select(ref m_LastClipPlayed, selectionMethod);

            var volume = m_Volume.Jitter(m_VolumeJitter) * volumeFactor;
            audioSource.pitch = m_Pitch.Jitter(m_PitchJitter);

            audioSource.PlayOneShot(clipToPlay, volume);
        }

        // Will use the AudioSource.PlayClipAtPoint() method, which doesn't include pitch variation.
        public void PlayAtPosition(Vector3 position, float volumeFactor = 1f, SelectionType selectionMethod = SelectionType.RandomExcludeLast)
        {
            if (!CanPlay())
                return;

            AudioClip clipToPlay = m_AudioClips.Select(ref m_LastClipPlayed, selectionMethod);

            if (clipToPlay != null)
                AudioSource.PlayClipAtPoint(clipToPlay, position, m_Volume.Jitter(m_VolumeJitter) * volumeFactor);
        }

        public void Play2D(float volumeFactor = 1f, SelectionType selectionMethod = SelectionType.RandomExcludeLast)
        {
            if (!CanPlay())
                return;

            AudioClip clipToPlay = m_AudioClips.Select(ref m_LastClipPlayed, selectionMethod);
            AudioManager.Play2D(clipToPlay, m_Volume.Jitter(m_VolumeJitter) * volumeFactor);
        }

        protected virtual bool CanPlay() => m_AudioClips.Length != 0;
    }
}