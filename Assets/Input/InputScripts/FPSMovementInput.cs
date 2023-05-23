using UnityEngine;
using UnityEngine.InputSystem;
using IdenticalStudios.MovementSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public class FPSMovementInput : CharacterInputBehaviour, IMovementInputProvider
    {
        public Vector2 RawMovementInput => m_MovementInputValue;
        public Vector3 MovementInput => Vector3.ClampMagnitude(m_Motor.transform.TransformVector(new Vector3(m_MovementInputValue.x, 0f, m_MovementInputValue.y)), 1f);

        public bool RunInput { get; private set; }
        public bool CrouchInput { get; private set; }
        public bool JumpInput { get; private set; }

        [SerializeField]
        private InputActionReference m_MoveInput;

        [SerializeField]
        private InputActionMode m_RunType;

        [SerializeField]
        private InputActionReference m_RunInput;

        [SerializeField]
        private InputActionMode m_CrouchType;

        [SerializeField]
        private InputActionReference m_CrouchInput;

        [SerializeField]
        private InputActionMode m_JumpType;

        [SerializeField]
        private float m_JumpReleaseDelay = 0.05f;

        [SerializeField]
        private InputActionReference m_JumpInput;

        private ICharacterMotor m_Motor;
        private Vector2 m_MovementInputValue;
        private float m_ReleaseJumpBtnTime;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_Motor);
        }

        protected override void OnInputEnabled()
        {
            ResetAllInputs();

            m_MoveInput.Enable();
            m_CrouchInput.Enable();
            m_RunInput.Enable();
            m_JumpInput.Enable();
        }

        protected override void OnInputDisabled()
        {
            ResetAllInputs();

            m_MoveInput.TryDisable();
            m_CrouchInput.TryDisable();
            m_RunInput.TryDisable();
            m_JumpInput.TryDisable();
        }
        #endregion

        #region Input Handling
        public void ResetAllInputs()
        {
            RunInput = false;
            JumpInput = false;
            m_ReleaseJumpBtnTime = 0f;
            m_MovementInputValue = Vector2.zero;
        }

        public void UseCrouchInput() => CrouchInput = false;
        public void UseRunInput() => RunInput = false;
        public void UseJumpInput() => JumpInput = false;

        protected override void TickInput()
        {
            // Handle movement input.
            m_MovementInputValue = m_MoveInput.action.ReadValue<Vector2>();

            // Handle run input.
            if (m_RunType == InputActionMode.Hold)
                RunInput = m_RunInput.action.ReadValue<float>() > 0.1f;
            else
            {
                if (m_RunInput.action.triggered)
                    RunInput = !RunInput;
            }

            // Handle crouch input.
            if (m_CrouchType == InputActionMode.Hold)
                CrouchInput = m_CrouchInput.action.ReadValue<float>() > 0.1f;
            else
            {
                if (m_CrouchInput.action.triggered)
                    CrouchInput = !CrouchInput;
            }

            // Handle jump input.
            if (m_JumpType == InputActionMode.Hold)
            {
                JumpInput = m_JumpInput.action.ReadValue<float>() > 0.1f;
            }
            else if (Time.time > m_ReleaseJumpBtnTime || !JumpInput)
            {
                JumpInput = m_JumpInput.action.triggered;

                if (JumpInput)
                    m_ReleaseJumpBtnTime = Time.time + m_JumpReleaseDelay;
            }
        }
        #endregion
    }
}