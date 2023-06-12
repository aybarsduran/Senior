using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class DelayedRandomSoundsEffect : WieldableEffect
    {
        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private DelayedSoundRandom[] m_Sounds;

        private IAudioPlayer m_AudioPlayer;


        public DelayedRandomSoundsEffect() => m_Sounds = Array.Empty<DelayedSoundRandom>();
        public DelayedRandomSoundsEffect(DelayedSoundRandom[] sounds) => m_Sounds = sounds;

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_AudioPlayer = wieldable.AudioPlayer;
        }

        public override void PlayEffect() => m_AudioPlayer.PlaySounds(m_Sounds);
        public override void PlayEffectDynamically(float value) => m_AudioPlayer.PlaySounds(m_Sounds, value);
    }
}
