using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CharacterIdleState : CharacterMotionState
    {
        public override MovementStateType StateType => MovementStateType.Idle;
        public override float StepCycleLength => 0f;
        public override bool ApplyGravity => false;
        public override bool SnapToGround => true;


        public override bool IsStateValid() => Motor.Velocity.GetHorizontal().sqrMagnitude < 0.01f && Motor.CanSetHeight(Motor.DefaultHeight);

        public override void OnStateEnter(MovementStateType prevStateType)
        {
            Input.UseRunInput();
            Motor.SetHeight(Motor.DefaultHeight);
        }

        public override void UpdateLogic()
        {
            // Transition to a walking state.
            if ((Input.RawMovementInput.sqrMagnitude > 0.1f || Motor.Velocity.GetHorizontal().sqrMagnitude > 0.01f) && Controller.TrySetState(MovementStateType.Walk)) return;

            // Transition to a jumping state.
            if (Input.JumpInput && Controller.TrySetState(MovementStateType.Jump)) return;

            // Transition to a crouched state.
            if (Input.CrouchInput && Controller.TrySetState(MovementStateType.Crouch)) return;

            // Transition to an airborne state.
            if (!Motor.IsGrounded && Controller.TrySetState(MovementStateType.Airborne)) return;
        }

        public override Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime) => Vector3.MoveTowards(currentVelocity, Vector3.zero, deltaTime);
    }
}