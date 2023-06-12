using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [System.Serializable]
    public sealed class SimpleSoundEffect : WieldableEffect
    {
        [SerializeField]
        private SimpleSound m_Sound;

        private IAudioPlayer m_AudioPlayer;


        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_AudioPlayer = wieldable.AudioPlayer;
        }

        public override void PlayEffect()
        {
#if UNITY_EDITOR
            if (m_AudioPlayer == null)
            {
                IsInitialized = false;
                return;
            }
#endif

            m_AudioPlayer.PlaySound(m_Sound);
        }
        public override void PlayEffectDynamically(float value) => m_AudioPlayer.PlaySound(m_Sound, value);
    }
}