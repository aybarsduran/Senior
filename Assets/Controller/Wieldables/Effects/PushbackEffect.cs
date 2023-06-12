using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class PushbackEffect : WieldableEffect
    {
        [SerializeField, Range(0f, 100f)]
        private float m_PushbackForce;

        private ICharacter m_Character;


        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_Character = wieldable.Character;
        }

        public override void PlayEffect()
        {
            if (m_PushbackForce > 0.01f)
            {
                if (m_Character.TryGetModule(out ICharacterMotor motor))
                    motor.AddForce(100f * m_PushbackForce * -m_Character.ViewTransform.forward, ForceMode.Impulse, true);
            }
        }

        public override void PlayEffectDynamically(float value)
        {
            if (m_PushbackForce > 0.01f)
            {
                if (m_Character.TryGetModule(out ICharacterMotor motor))
                    motor.AddForce(100f * m_PushbackForce * value * -m_Character.ViewTransform.forward, ForceMode.Impulse, true);
            }
        }
    }
}
