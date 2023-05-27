using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.MovementSystem
{ 
    public sealed class CharacterJumpBlocker : CharacterBehaviour
    {
        [SerializeField, Range(0f, 0.5f)]
        //At which stamina value (0-1) will the ability to jump be disabled.
        private float m_DisableJumpOnStaminaValue = 0.1f;

        [SerializeField, Range(0f, 0.5f)]
        //At which stamina value (0-1) will the ability to jump be re-Enabled (if disabled)
        private float m_EnableJumpOnStaminaValue = 0.3f;

        private IMovementController m_Movement;
        private IStaminaController m_Stamina;

        private bool m_JumpDisabled;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Movement);
            GetModule(out m_Stamina);

            m_Stamina.StaminaChanged += OnStaminaChanged;
        }

        private void OnStaminaChanged(float stamina)
        {
            if (!m_JumpDisabled && stamina < m_DisableJumpOnStaminaValue)
            {
                m_Movement.AddStateLocker(this, MovementStateType.Jump);
                m_JumpDisabled = true;
            }
            else if (m_JumpDisabled && stamina > m_EnableJumpOnStaminaValue)
            {
                m_Movement.RemoveStateLocker(this, MovementStateType.Jump);
                m_JumpDisabled = false;
            }
        }
    }
}