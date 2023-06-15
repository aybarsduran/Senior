using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CharacterSlideState : CharacterMotionState
    {
        public override MovementStateType StateType => MovementStateType.Slide;
        public override bool ApplyGravity => false;
        public override bool SnapToGround => true;
        public override float StepCycleLength => Mathf.Infinity;

        [Tooltip("Sliding speed over time.")]
        [SerializeField]
        private AnimationCurve m_SlideSpeed;

        [SerializeField, Range(0f, 100f)]
        private float m_SlideImpulse = 10f;

        [SerializeField, Range(0f, 100f)]
        [Tooltip("How fast will the character stop when there's no input (a high value will make the movement feel snappier).")]
        private float m_SlideAcceleration = 10f;

        [Space]

        [Tooltip("The controllers height when sliding.")]
        [SerializeField, Range(0f, 2f)]
        private float m_SlideHeight = 1f;

        [SerializeField]
        private AnimationCurve m_SlideEnterSpeed;

        [SerializeField, Range(0f, 25f)]
        private float m_MinSlideSpeed = 4f;

        [SerializeField, Range(0.1f, 10f)]
        private float m_SlideStopSpeed = 2f;

        [Space]

        [Tooltip("How much control does the Player input have on the slide direction.")]
        [SerializeField, Range(0f, 10f)]
        private float m_InputFactor;

        [SerializeField, Range(0f, 50f)]
        private float m_SlopeFactor = 2f;

        private Vector3 m_SlideDirection;
        private float m_SlideStartTime;
        private float m_InitialSlideImpulseMod;
        private bool m_Jump;


        protected override void OnStateInitialized()
        {
            m_SlideDirection = Vector3.zero;
            m_InitialSlideImpulseMod = 0f;
            m_SlideStartTime = 0f;
        }

        public override bool IsStateValid()
        {
            Vector3 motorVelocity = Motor.Velocity;

            bool canSlide =
                new Vector2(motorVelocity.x, motorVelocity.z).magnitude > m_MinSlideSpeed &&
                Motor.IsGrounded &&
                Motor.CanSetHeight(m_SlideHeight);

            return canSlide;
        }

        public override void OnStateEnter(MovementStateType prevStateType)
        {
            m_SlideDirection = Vector3.ClampMagnitude(Motor.SimulatedVelocity, 1f);
            m_SlideStartTime = Time.time;
            Motor.SetHeight(m_SlideHeight);

            m_InitialSlideImpulseMod = m_SlideEnterSpeed.Evaluate(Motor.Velocity.magnitude) * m_SlideImpulse;

            if (prevStateType == MovementStateType.Airborne)
                m_InitialSlideImpulseMod *= 0.33f;

            m_Jump = false;
            Input.UseRunInput();
        }

        public override void UpdateLogic()
        {
            // Transition to airborne state.
            if (!Motor.IsGrounded && Controller.TrySetState(MovementStateType.Airborne)) return;

            if (Motor.Velocity.magnitude < m_SlideStopSpeed)
            {
                // Transition to a running state.
                if (Input.RunInput && Controller.TrySetState(MovementStateType.Run)) return;

                // Transition to a crouch state.
                if ((Motor.Velocity.sqrMagnitude < 2f || Input.RawMovementInput.sqrMagnitude > 0.1f) && Controller.TrySetState(MovementStateType.Crouch)) return;
            }

            if (Input.JumpInput)
                m_Jump = true;

            // Transition to a jump state.
            if (m_Jump && m_SlideStartTime + 0.2f < Time.time && Controller.TrySetState(MovementStateType.Jump)) return;
        }

        public override Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime)
        {
            // Calculate the sliding impulse velocity.
            Vector3 targetVelocity = m_SlideDirection * (m_SlideSpeed.Evaluate(Time.time - m_SlideStartTime) * m_InitialSlideImpulseMod);

            // Sideways movement.
            if (Mathf.Abs(Input.RawMovementInput.x) > 0.01f)
                targetVelocity += Vector3.ClampMagnitude(Motor.transform.TransformVector(new Vector3(Input.RawMovementInput.x, 0f, 0f)), 1f);

            // Combine the target velocity with the sideways movement.
            float previousMagnitude = targetVelocity.magnitude;
            targetVelocity = Vector3.ClampMagnitude((Input.MovementInput * m_InputFactor) + targetVelocity, previousMagnitude);

            // Make sure to increase the speed when descending steep surfaces.
            float surfaceAngle = Motor.GroundSurfaceAngle;
            if (surfaceAngle > 3f)
            {
                bool isDescendingSlope = Vector3.Dot(Motor.GroundNormal, Motor.SimulatedVelocity) > 0f;
                float slope = Mathf.Min(surfaceAngle, Motor.SlopeLimit) / Motor.SlopeLimit;
                Vector3 slopeDirection = Motor.GroundNormal;
                slopeDirection.y = 0f;

                // Increase the sliding force when going down slopes.
                if (isDescendingSlope)
                    currentVelocity += slopeDirection * (slope * m_SlopeFactor * deltaTime * 10f);
            }

            // Get the velocity mod.
            float velocityMod = Controller.VelocityModifier.Get() * Motor.GetSlopeSpeedMultiplier();

            // Get the acceleration mod.
            float acceleration = targetVelocity.sqrMagnitude > 0.1f ? Controller.AccelerationModifier.Get() : Controller.DecelerationModifier.Get();

            // Lower velocity if the motor has collided with anything.
            if (Motor.CollisionFlags.Has(CollisionFlags.CollidedSides))
                velocityMod *= 0.5f;

            // Finally multiply the target velocity with the velocity modifier.
            targetVelocity *= velocityMod;

            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, deltaTime * m_SlideAcceleration * acceleration);

            return currentVelocity;
        }
    }
}