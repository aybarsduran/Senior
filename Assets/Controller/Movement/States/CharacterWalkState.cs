namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CharacterWalkState : CharacterGroundedState
    {
        public override MovementStateType StateType => MovementStateType.Walk;


        public override bool IsStateValid() => Motor.IsGrounded && Motor.CanSetHeight(Motor.DefaultHeight);
        public override void OnStateEnter(MovementStateType prevStateType) => Motor.SetHeight(Motor.DefaultHeight);

        public override void UpdateLogic()
        {
            // Transition to an idle state.
            if (((Input.RawMovementInput.sqrMagnitude < 0.1f && Motor.SimulatedVelocity.sqrMagnitude < 0.01f) || !Enabled)
                && Controller.TrySetState(MovementStateType.Idle)) return;

            // Transition to a run state.
            if (Input.RunInput && Controller.TrySetState(MovementStateType.Run)) return;

            // Transition to a crouch state.
            if (Input.CrouchInput && Controller.TrySetState(MovementStateType.Crouch)) return;

            // Transition to an airborne state.
            if (!Motor.IsGrounded && Controller.TrySetState(MovementStateType.Airborne)) return;

            // Transition to a jumping state.
            if (Input.JumpInput && Controller.TrySetState(MovementStateType.Jump)) return;
        }
    }
}