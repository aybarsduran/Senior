using IdenticalStudios;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    
    public sealed class AudioPlayer : CharacterBehaviour, IAudioPlayer
    {
        public struct QueuedSound
        {
            public Sound Sound { get; }
            public float Volume { get; }
            public float StopTime { get; }
            public int AudioSourceIndex { get; }


            public QueuedSound(Sound sound, int audioSourceIndex, float volume = 1f)
            {
                this.Sound = sound;
                this.Volume = volume;
                this.StopTime = Time.time + sound.Delay;
                this.AudioSourceIndex = audioSourceIndex;
            }

            public QueuedSound(Sound sound, int audioSourceIndex, float speed, float volume = 1f)
            {
                this.Sound = sound;
                this.Volume = volume;
                this.StopTime = Time.time + sound.Delay / speed;
                this.AudioSourceIndex = audioSourceIndex;
            }
        }

        public class LoopingQueuedSound
        {
            public Sound Sound { get; }
            public float StopTime { get; set; }
            public float LoopVolume { get; }
            public int AudioSourceIndex { get; }


            public LoopingQueuedSound(Sound sound, float duration, int audioSourceIndex)
            {
                this.Sound = sound;
                this.StopTime = Time.time + duration;
                this.LoopVolume = sound.Volume;
                this.AudioSourceIndex = audioSourceIndex;
            }
        }

        [SerializeField]
        private Transform m_PlayRoot;


        [SerializeField, Range(0, 24)]
        private int m_MainAudioSourcesCount = 6;

        [SerializeField, Range(0, 24)]
        private int m_LoopingAudioSourcesCount = 6;

        private readonly List<QueuedSound> m_QueuedSounds = new List<QueuedSound>();
        private readonly List<AudioSource> m_MainAudioSources = new List<AudioSource>();
        private readonly List<AudioSource> m_LoopingAudioSources = new List<AudioSource>();
        private readonly List<LoopingQueuedSound> m_LoopingQueuedSounds = new List<LoopingQueuedSound>();

        private int m_MainAudioSourceIndex = -1;
        private int m_LoopingAudioSourceIndex = -1;


        public void PlaySound(Sound sound, float volume = 1f)
        {
            if (sound == null || m_MainAudioSourcesCount <= 0)
                return;

            IncrementMainAudioSourcesIndex();

            if (sound.Delay > 0.03f)
                m_QueuedSounds.Add(new QueuedSound(sound, m_MainAudioSourceIndex));
            else
            {
                if (sound.AudioClip == null)
                    return;

                m_MainAudioSources[m_MainAudioSourceIndex].pitch = sound.Pitch;
                m_MainAudioSources[m_MainAudioSourceIndex].PlayOneShot(sound.AudioClip, sound.Volume * volume);
            }
        }

        public void PlaySound(Sound sound, float speed, float volume = 1f)
        {
            if (sound == null || m_MainAudioSourcesCount <= 0)
                return;

            IncrementMainAudioSourcesIndex();

            if (sound.Delay > 0.03f)
                m_QueuedSounds.Add(new QueuedSound(sound, m_MainAudioSourceIndex, speed));
            else
            {
                var audioClip = sound.AudioClip;

                if (audioClip == null)
                    return;

                m_MainAudioSources[m_MainAudioSourceIndex].pitch = sound.Pitch;
                m_MainAudioSources[m_MainAudioSourceIndex].PlayOneShot(audioClip, sound.Volume);
            }
        }

        public void PlaySounds(Sound[] sounds, float volume = 1f)
        {
            if (sounds == null)
                return;

            for (int i = 0; i < sounds.Length; i++)
                PlaySound(sounds[i]);
        }

        public void PlaySounds(Sound[] sounds, float speed, float volume = 1f)
        {
            if (sounds == null)
                return;

            for (int i = 0; i < sounds.Length; i++)
                PlaySound(sounds[i], speed, volume);
        }

        public void LoopSound(Sound sound, float duration)
        {
            if (sound == null || m_LoopingAudioSourcesCount <= 0)
                return;

            IncrementLoopingAudioSourcesIndex();

            if (duration < Mathf.Epsilon)
                duration = float.MaxValue;

            m_LoopingQueuedSounds.Add(new LoopingQueuedSound(sound, duration, m_LoopingAudioSourceIndex));
        }

        public void ClearAllQueuedSounds() => m_QueuedSounds.Clear();

        public void StopLoopingSound(Sound sound, float stopTime = 0.5f)
        {
            if (sound == null)
                return;

            foreach (var snd in m_LoopingQueuedSounds)
            {
                if (snd.Sound == sound)
                    snd.StopTime = Time.time + stopTime;
            }
        }

        public void StopAllLoopingSounds()
        {
            float stopTime = Time.time + 0.3f;

            foreach (var sound in m_LoopingQueuedSounds)
                sound.StopTime = stopTime;
        }

        protected override void OnBehaviourEnabled()
        {
            for (int i = 0; i < m_MainAudioSourcesCount; i++)
            {
                m_MainAudioSources.Add(AudioManager.CreateAudioSource(m_PlayRoot.gameObject, false, 1f, 1f, AudioManager.MixerOutputGroup.Effects));
                m_MainAudioSources[i].loop = false;
            }

            for (int i = 0; i < m_LoopingAudioSourcesCount; i++)
            {
                m_LoopingAudioSources.Add(AudioManager.CreateAudioSource(m_PlayRoot.gameObject, false, 1f, 1f, AudioManager.MixerOutputGroup.Effects));
                m_LoopingAudioSources[i].loop = true;
            }

            Character.HealthManager.Death += StopAllLoopingSounds;
        }

        private void Update()
        {
            if (m_QueuedSounds.Count > 0)
                UpdateDelayedSounds();

            if (m_LoopingQueuedSounds.Count > 0)
                UpdateLoopingSounds();
        }

        private void UpdateDelayedSounds()
        {
            for (int i = 0; i < m_QueuedSounds.Count; i++)
            {
                QueuedSound queuedSound = m_QueuedSounds[i];

                if (!(Time.time >= queuedSound.StopTime))
                    continue;

                Sound sound = queuedSound.Sound;

                m_MainAudioSources[queuedSound.AudioSourceIndex].pitch = sound.Pitch;
                m_MainAudioSources[queuedSound.AudioSourceIndex].PlayOneShot(sound.AudioClip, sound.Volume * queuedSound.Volume);

                m_QueuedSounds.RemoveAt(i);
            }
        }

        private void UpdateLoopingSounds()
        {
            for (int i = 0; i < m_LoopingQueuedSounds.Count; i++)
            {
                AudioSource loopingAudioSource = m_LoopingAudioSources[m_LoopingQueuedSounds[i].AudioSourceIndex];

                if (!loopingAudioSource.isPlaying)
                {
                    loopingAudioSource.clip = m_LoopingQueuedSounds[i].Sound.AudioClip;
                    loopingAudioSource.pitch = m_LoopingQueuedSounds[i].Sound.Pitch;
                    loopingAudioSource.Play();
                }

                // Fade in volume
                if (Time.time < m_LoopingQueuedSounds[i].StopTime)
                    loopingAudioSource.volume = Mathf.MoveTowards(loopingAudioSource.volume, m_LoopingQueuedSounds[i].LoopVolume, Time.deltaTime * 2f);
                // Fade out volume
                else
                {
                    loopingAudioSource.volume = Mathf.MoveTowards(loopingAudioSource.volume, 0f, Time.deltaTime * 2f);

                    if (!(loopingAudioSource.volume < 0.01f))
                        continue;

                    loopingAudioSource.Stop();
                    loopingAudioSource.clip = null;

                    m_LoopingQueuedSounds.RemoveAt(i);
                }
            }
        }

        private void IncrementMainAudioSourcesIndex() => m_MainAudioSourceIndex = (int)Mathf.Repeat(m_MainAudioSourceIndex + 1, m_MainAudioSourcesCount - 1);
        private void IncrementLoopingAudioSourcesIndex() => m_LoopingAudioSourceIndex = (int)Mathf.Repeat(m_LoopingAudioSourceIndex + 1, m_LoopingAudioSourcesCount - 1);
    }
}