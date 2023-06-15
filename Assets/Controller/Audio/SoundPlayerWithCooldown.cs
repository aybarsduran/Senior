using System;
using UnityEngine;

namespace IdenticalStudios
{
    [Serializable]
    public class SoundPlayerWithCooldown : SoundPlayer
    {
        [SerializeField, Range(0f, 10f)]
        private float m_Cooldown = 0.5f;

        private float m_NextTimeCanPlay;


        public void SetCooldown(float cooldown) => m_NextTimeCanPlay = Time.time + cooldown;

        protected override bool CanPlay()
        {
            bool canPlay = base.CanPlay() && m_NextTimeCanPlay < Time.time;

            if (canPlay)
                m_NextTimeCanPlay = Time.time + m_Cooldown;

            return canPlay;
        }
    }
}
