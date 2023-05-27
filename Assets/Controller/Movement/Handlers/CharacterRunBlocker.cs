using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    public sealed class CharacterRunBlocker : CharacterBehaviour
    {
        [SerializeField, Range(0f, 0.5f)]
        //At which stamina value (0-1) will the ability to run be disabled.")]
        private float m_DisableRunOnStaminaValue = 0.1f;

        [SerializeField, Range(0f, 0.5f)]
        //At which stamina value (0-1) will the ability to run be re-Enabled (if disabled)
        private float m_EnableRunOnStaminaValue = 0.3f;

        private IMovementController m_Movement;
        private IStaminaController m_Stamina;

        private bool m_RunDisabled;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Movement);
            GetModule(out m_Stamina);

            m_Stamina.StaminaChanged += OnStaminaChanged;
        }

        private void OnStaminaChanged(float stamina)
        {
            if (!m_RunDisabled && stamina < m_DisableRunOnStaminaValue)
            {
                m_Movement.AddStateLocker(this, MovementStateType.Run);
                m_RunDisabled = true;
            }
            else if (m_RunDisabled && stamina > m_EnableRunOnStaminaValue)
            {
                m_Movement.RemoveStateLocker(this, MovementStateType.Run);
                m_RunDisabled = false;
            }
        }
    }
}