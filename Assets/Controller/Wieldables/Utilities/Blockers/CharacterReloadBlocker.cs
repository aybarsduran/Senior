using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class CharacterReloadBlocker : WieldableInputBlocker
    {

        [SerializeField]
        private bool m_BlockRunning;

        [SerializeField]
        private bool m_BlockJumping;

        [SerializeField]
        private bool m_ReloadWhileRunning;

        [SerializeField]
        private bool m_ReloadWhileAirborne;

        private IReloadInputHandler m_ReloadInputHandler;
        private IMovementController m_Movement;
        private ICharacterMotor m_Motor;
        private bool m_WasReloading = false;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Movement);
            GetModule(out m_Motor);

            base.OnBehaviourEnabled();
        }

        protected override ActionBlockHandler GetBlockHandler(IWieldable wieldable)
        {
            if (wieldable is IReloadInputHandler input)
            {
                m_ReloadInputHandler = input;
                return input.ReloadBlocker;
            }

            m_ReloadInputHandler = null;
            return null;
        }

        protected override bool IsInputValid()
        {
            bool isValid = (m_ReloadWhileAirborne || m_Motor.IsGrounded) &&
                            m_Movement.ActiveState != MovementStateType.Run || m_ReloadWhileRunning;

            return isValid;
        }

        protected override void OnUpdate()
        {
            if (!m_WasReloading && m_ReloadInputHandler.IsReloading)
            {
                if (m_BlockRunning)
                    m_Movement.AddStateLocker(this, MovementStateType.Run);

                if (m_BlockJumping)
                    m_Movement.AddStateLocker(this, MovementStateType.Jump);
            }
            else if (m_WasReloading && !m_ReloadInputHandler.IsReloading)
            {
                if (m_BlockRunning)
                    m_Movement.RemoveStateLocker(this, MovementStateType.Run);

                if (m_BlockJumping)
                    m_Movement.RemoveStateLocker(this, MovementStateType.Jump);
            }

            m_WasReloading = m_ReloadInputHandler.IsReloading;
        }
    }
}