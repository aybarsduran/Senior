using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class CharacterUseBlocker : WieldableInputBlocker
    {
        [Title("Settings")]

        [SerializeField]
        private bool m_UseWhileAirborne;

        [SerializeField]
        private bool m_UseWhileRunning;

        private IMovementController m_Movement;
        private ICharacterMotor m_Motor;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Movement);
            GetModule(out m_Motor);

            base.OnBehaviourEnabled();
        }

        protected override ActionBlockHandler GetBlockHandler(IWieldable wieldable)
        {
            if (wieldable is IUseInputHandler input)
                return input.UseBlocker;

            return null;
        }

        protected override bool IsInputValid()
        {
            bool isValid = (m_UseWhileAirborne || m_Motor.IsGrounded) &&
                           (m_UseWhileRunning || m_Movement.ActiveState != MovementStateType.Run);

            return isValid;
        }
    }
}