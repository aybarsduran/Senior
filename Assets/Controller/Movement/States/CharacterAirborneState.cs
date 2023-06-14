using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CharacterAirborneState : CharacterMotionState
    {
        public override MovementStateType StateType => MovementStateType.Airborne; 
        public override float StepCycleLength => 1f;
        public override bool ApplyGravity => true;
        public override bool SnapToGround => false;

        [SerializeField, Range(0f, 10f)]
        private float m_MinAirborneControl = 5f;

        [SerializeField, Range(0f, 10f)]
        private float m_MaxAirborneControl = 5f;

        [SerializeField, Range(0.1f, 10f)]
        private float m_AirborneAcceleration = 3f;

        [SerializeField, Range(0f, 5f)]
        private float m_LandVelocityMod = 0.8f;

        private Vector3 m_OriginalHorizontalVelocity;
        private Vector3 m_CurrentInput;
        private float m_MaxSpeed;


        public override bool IsStateValid() => !Motor.IsGrounded;

        public override void UpdateLogic()
        {
            // Try double, triple... jump.
            if (Input.JumpInput && Controller.TrySetState(MovementStateType.Jump)) return;

            if (Motor.IsGrounded)
            {
                // Transition to a run state.
                if (Input.RunInput && Controller.TrySetState(MovementStateType.Run)) return;

                // Transition to a slide or crouch state.
                if (Input.CrouchInput && (Controller.TrySetState(MovementStateType.Slide))) return;

                // Transition to an idle state.
                if (Controller.TrySetState(MovementStateType.Idle)) return;

                // Transition to a walking state.
                if (Controller.TrySetState(MovementStateType.Walk)) return;
            }

            if (Input.RawMovementInput.sqrMagnitude < 0.01f)
                Input.UseCrouchInput();
        }

        public override void OnStateEnter(MovementStateType prevStateType)
        {
            m_MaxSpeed = Mathf.Max(Motor.SimulatedVelocity.GetHorizontal().magnitude, m_MinAirborneControl);
            m_OriginalHorizontalVelocity = Motor.SimulatedVelocity.GetHorizontal();
            m_CurrentInput = Vector3.zero;
        }

        public override Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime)
        {
            // Modify the current velocity by taking into account how well we can change direction when not grounded (see "m_AirControl" tooltip).
            m_CurrentInput = Vector3.Lerp(m_CurrentInput, m_MaxAirborneControl * Input.MovementInput, deltaTime * m_AirborneAcceleration);
            var horizVelocity = m_CurrentInput + m_OriginalHorizontalVelocity;
            horizVelocity = Vector3.ClampMagnitude(horizVelocity, m_MaxSpeed);

            if (Motor.CollisionFlags.Has(CollisionFlags.CollidedAbove) && currentVelocity.y > 0.1f)
                currentVelocity.y = -currentVelocity.y * 0.5f;

            // Apply a velocity mod on landing.
            if (Motor.IsGrounded)
                horizVelocity *= m_LandVelocityMod;

            return new Vector3(horizVelocity.x, currentVelocity.y, horizVelocity.z);
        }
    }
}