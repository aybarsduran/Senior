using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class OneShotSoundEffect : WieldableEffect
    {
        [SerializeField] 
        private StandardSound m_Sound;

        private IAudioPlayer m_AudioPlayer;


        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_AudioPlayer = wieldable.AudioPlayer;
        }

        public override void PlayEffect() => m_AudioPlayer.PlaySound(m_Sound);
        public override void PlayEffectDynamically(float value) => m_AudioPlayer.PlaySound(m_Sound, value);
    }
}