using System;
using UnityEngine;

namespace IdenticalStudios
{
    public interface ISound
    {
        AudioClip AudioClip { get; }
        float Volume { get; }
        float Pitch { get; }
        float Delay { get; }
    }

    public abstract class Sound
    {
        public abstract AudioClip AudioClip { get; }
        public abstract float Volume { get; }
        public abstract float Pitch { get; }
        public abstract float Delay { get; }
    }

    [Serializable]
    public class StandardSoundWithCooldown : StandardSound
    {
        public override float Volume
        {
            get 
            {
                bool canPlay = m_NextTimeCanPlay < Time.time;

                if (canPlay)
                {
                    m_NextTimeCanPlay = Time.time + m_Cooldown;
                    return base.Volume;
                }

                return 0f;
            }
        }

        [Space]
        [SerializeField, Range(0f, 10f)]
        private float m_Cooldown = 0.5f;

        private float m_NextTimeCanPlay;
        
        
        public void SetCooldown(float cooldown) => m_NextTimeCanPlay = Time.time + cooldown;
    }

    [Serializable]
    public class StandardSound : Sound, ISound
    {
        public override AudioClip AudioClip => m_AudioClips.Select(ref m_LastPlayedClip, SelectionType.Random);
        public override float Volume => m_Volume.Jitter(m_VolumeJitter);
        public override float Pitch => m_Pitch.Jitter(m_PitchJitter);
        public override float Delay => 0f;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.65f;

        [SerializeField, Range(0f, 1f)]
        private float m_Pitch = 1f;

        [SerializeField, Range(0f, 1f)]
        private float m_VolumeJitter = 0.1f;
        
        [SerializeField, Range(0f, 1f)]
        private float m_PitchJitter = 0.1f;

        [SerializeField, ReorderableList(ListStyle.Lined, HasLabels = false)]
        private AudioClip[] m_AudioClips;

        private int m_LastPlayedClip = -1;
    }

    [Serializable]
    public class SimpleSound : Sound, ISound
    {
        public override AudioClip AudioClip => m_Clip;
        public override float Volume => m_Volume;
        public override float Pitch => 1f;
        public override float Delay => 0f;

        [SerializeField, InLineEditor]
        private AudioClip m_Clip;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.5f;
    }

    [Serializable]
    public class DelayedSoundRandom : Sound, ISound
    {
        public override AudioClip AudioClip => m_AudioClips.Select(ref m_LastPlayedClip, SelectionType.RandomExcludeLast);
        public override float Volume => m_Volume;
        public override float Pitch => 1f;
        public override float Delay => m_Delay;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.5f;

        [SerializeField, Range(0f, 20f)]
        private float m_Delay;

        [SerializeField, ReorderableList(ListStyle.Lined, HasLabels = false)]
        private AudioClip[] m_AudioClips;

        private int m_LastPlayedClip = -1;
    }

    [Serializable]
    public class DelayedSound : Sound, ISound
    {
        public override AudioClip AudioClip => m_Clip;
        public override float Volume => m_Volume;
        public override float Pitch => 1f;
        public override float Delay => m_Delay;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.5f;

        [SerializeField, Range(0f, 20f)]
        private float m_Delay;
        
        [SerializeField, InLineEditor]
        private AudioClip m_Clip;
    }
}
