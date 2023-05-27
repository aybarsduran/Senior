using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    public sealed class CharacterRunState : CharacterGroundedState
    {
        public override MovementStateType StateType => MovementStateType.Run;

        [Space]
        [SerializeField, Range(0f, 20f)]
        private float m_MinRunSpeed = 3f;


        public override bool IsStateValid()
        {
            Vector2 rawMoveInput = Input.RawMovementInput;
            bool noInput = rawMoveInput == Vector2.zero;
            bool wantsToMoveBack = rawMoveInput.y < 0f;
            bool wantsToMoveOnlySideways = Mathf.Abs(rawMoveInput.x) > 0.9f;

            if (noInput || wantsToMoveBack || wantsToMoveOnlySideways)
                return false;

            var velocity = Motor.Velocity;
            var hVelocity = new Vector2(velocity.x, velocity.z);

            bool canRun = rawMoveInput.sqrMagnitude > 0.1f && hVelocity.magnitude > m_MinRunSpeed
                        && Motor.CanSetHeight(Motor.DefaultHeight) && Motor.IsGrounded;

            return canRun;
        }

        public override void OnStateEnter(MovementStateType prevStateType)
        {
            base.OnStateEnter(prevStateType);

            Motor.SetHeight(Motor.DefaultHeight);
            Input.UseCrouchInput();
        }

        public override void UpdateLogic()
        {
            if (Enabled)
            {
                // Transition to an airborne state.
                if (Controller.TrySetState(MovementStateType.Airborne)) return;

                // Transition to a walk state.
                if ((!Input.RunInput || !IsStateValid()) && Controller.TrySetState(MovementStateType.Walk)) return;

                // Transition to a slide or crouch state.
                if (Input.CrouchInput && (Controller.TrySetState(MovementStateType.Slide) || Controller.TrySetState(MovementStateType.Crouch))) return;

                // Transition to a jumping state.
                if (Input.JumpInput && Controller.TrySetState(MovementStateType.Jump)) return;
            }
            else
            {
                // Transition to a walk state.
                if (Controller.TrySetState(MovementStateType.Walk)) return;

                // Transition to an idle state.
                Controller.TrySetState(MovementStateType.Idle);
            }
        }
    }
}