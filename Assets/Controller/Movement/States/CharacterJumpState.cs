using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CharacterJumpState : CharacterMotionState
    {
        public override MovementStateType StateType => MovementStateType.Jump;
        public override float StepCycleLength => 1f;
        public override bool ApplyGravity => false;
        public override bool SnapToGround => false;

        public int MaxJumpsCount
        {
            get => m_MaxJumpsCount;
            set
            {
                m_MaxJumpsCount = value;

                if (Motor.IsGrounded)
                    ResetJumpsCount();
            }
        }

        public int DefaultJumpsCount => m_JumpsCount; 

        [Tooltip("The max height of a jump.")]
        [SerializeField, Range(0.1f, 10f)]
        private float m_JumpHeight = 1f;

        [SerializeField, Range(1, 10)]
        private int m_JumpsCount;

        [Tooltip("How often can this character jump (in seconds).")]
        [SerializeField, Range(0f, 1.5f)]
        private float m_JumpCooldown = 0.3f;

        [SerializeField, Range(0f, 1f)]
        private float m_CoyoteJumpTime = 0.1f;

        private float m_NextTimeCanJump;
        private int m_JumpsCountLeft;
        private int m_MaxJumpsCount;


        protected override void OnStateInitialized()
        {
            Motor.GroundedChanged += OnGroundedChanged;

            m_MaxJumpsCount = m_JumpsCount;
            m_NextTimeCanJump = -1f;
            ResetJumpsCount();
        }

        protected override void OnStateDecommisioned()
        {
            Motor.GroundedChanged -= OnGroundedChanged;
        }

        public void ResetJumpsCount() => m_JumpsCountLeft = m_MaxJumpsCount;

        public override bool IsStateValid()
        {
            if (!Motor.CanSetHeight(Motor.DefaultHeight))
                return false;

            if (m_JumpsCountLeft == m_MaxJumpsCount && !IsGrounded())
                m_JumpsCountLeft--;

            return m_JumpsCountLeft > 0 && m_NextTimeCanJump < Time.time;
        }

        public override void OnStateEnter(MovementStateType prevStateType)
        {
            Motor.SetHeight(Motor.DefaultHeight);
            m_JumpsCountLeft--;

            Input.UseCrouchInput();
            Input.UseJumpInput();
        }

        public override void UpdateLogic()
        {
            // Transition to airborne state.
            if (Controller.TrySetState(MovementStateType.Airborne))
                return;
        }

        public override Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime)
        {
            float jumpHeight = m_JumpHeight;

            if (jumpHeight > 0.1f)
            {
                float jumpSpeed = Mathf.Sqrt(2 * Motor.Gravity * jumpHeight);
                currentVelocity = new Vector3(currentVelocity.x, jumpSpeed, currentVelocity.z);
            }

            return currentVelocity;
        }

        private void OnGroundedChanged(bool grounded)
        {
            if (grounded)
            {
                m_NextTimeCanJump = Time.time + m_JumpCooldown;
                ResetJumpsCount();
            }
        }

        private bool IsGrounded() => (Motor.LastGroundedChangeTime + m_CoyoteJumpTime > Time.time) || Motor.IsGrounded;

        #region Editor
#if UNITY_EDITOR
        public override void OnEditorValidate()
        {
            if (Application.isPlaying && Motor != null)
            {
                m_MaxJumpsCount = m_JumpsCount;
                ResetJumpsCount();
            }
        }
#endif
        #endregion
    }
}