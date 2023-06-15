using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class CharacterAimBlocker : WieldableInputBlocker
    {
        [Title("Settings")]

        [BeginHorizontal]
        [SerializeField, LeftToggle]
        private bool m_BlockRunning = true;

        [EndHorizontal]
        [SerializeField, LeftToggle]
        private bool m_BlockJumping;

        [BeginHorizontal]
        [SerializeField, LeftToggle]
        private bool m_AimWhileRunning = true;

        [EndHorizontal]
        [SerializeField, LeftToggle]
        private bool m_AimWhileAirborne;

        private IAimInputHandler m_AimInputHandler;
        private IMovementController m_Movement;
        private ICharacterMotor m_Motor;
        private bool m_WasAiming;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Motor);
            GetModule(out m_Movement);

            base.OnBehaviourEnabled();
        }

        protected override ActionBlockHandler GetBlockHandler(IWieldable wieldable)
        {
            if (wieldable is IAimInputHandler input)
            {
                m_AimInputHandler = input;
                return input.AimBlocker;
            }

            m_AimInputHandler = null;
            return null;
        }

        protected override bool IsInputValid()
        {
            bool isValid = (m_AimWhileAirborne || m_Motor.IsGrounded) &&
                           (m_AimWhileRunning || m_Movement.ActiveState != MovementStateType.Run);

            return isValid;
        }

        protected override void OnUpdate()
        {
            if (!m_WasAiming && m_AimInputHandler.IsAiming)
            {
                if (m_BlockRunning)
                    m_Movement.AddStateLocker(this, MovementStateType.Run);

                if (m_BlockJumping)
                    m_Movement.AddStateLocker(this, MovementStateType.Jump);
            }
            else if (m_WasAiming && !m_AimInputHandler.IsAiming)
            {
                if (m_BlockRunning)
                    m_Movement.RemoveStateLocker(this, MovementStateType.Run);

                if (m_BlockJumping)
                    m_Movement.RemoveStateLocker(this, MovementStateType.Jump);
            }

            m_WasAiming = m_AimInputHandler.IsAiming;
        }
    }
}
