using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class DelayedSoundsEffect : WieldableEffect
    {
        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private DelayedSound[] m_Sounds;

        private IAudioPlayer m_AudioPlayer;


        public DelayedSoundsEffect() => m_Sounds = Array.Empty<DelayedSound>();
        public DelayedSoundsEffect(DelayedSound[] sounds) => m_Sounds = sounds;

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_AudioPlayer = wieldable.AudioPlayer;
        }

        public override void PlayEffect() => m_AudioPlayer.PlaySounds(m_Sounds);
        public override void PlayEffectDynamically(float value) => m_AudioPlayer.PlaySounds(m_Sounds, value); 
    }
}