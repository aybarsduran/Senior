using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CharacterCrouchState : CharacterGroundedState
    {
        public override MovementStateType StateType => MovementStateType.Crouch;

        [Space]
        [Tooltip("The controllers height when crouching.")]
        [SerializeField, Range(0f, 2f)]
        private float m_CrouchHeight = 1f;

        [Tooltip("How long does it take to crouch.")]
        [SerializeField, Range(0f, 1f)]
        private float m_CrouchCooldown = 0.3f;

        private float m_NextTimeCanCrouch = 0f;


        protected override void OnStateInitialized()
        {
            m_NextTimeCanCrouch = 0f;
        }

        public override bool IsStateValid()
        {
            bool canCrouch =
                Time.unscaledTime > m_NextTimeCanCrouch &&
                Motor.IsGrounded &&
                Motor.CanSetHeight(m_CrouchHeight);

            return canCrouch;
        }

        public override void OnStateEnter(MovementStateType prevStateType)
        {
            m_NextTimeCanCrouch = Time.unscaledTime + m_CrouchCooldown;
            Motor.SetHeight(m_CrouchHeight);
        }

        public override void UpdateLogic()
        {
            // Transition to an airborne state.
            if (Controller.TrySetState(MovementStateType.Airborne)) return;

            // Transition to an idle or walk state.
            if (Time.unscaledTime > m_NextTimeCanCrouch + m_CrouchCooldown && (!Input.CrouchInput || Input.JumpInput || Input.RunInput))
            {
                Input.UseCrouchInput();
                Input.UseJumpInput();
                Controller.TrySetState(Motor.SimulatedVelocity.sqrMagnitude > 0.1f ? MovementStateType.Walk : MovementStateType.Idle);
            }
        }

        public override void OnStateExit()
        {
            m_NextTimeCanCrouch = Time.unscaledTime + m_CrouchCooldown;
        }
    }
}