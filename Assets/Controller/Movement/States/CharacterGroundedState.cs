using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public abstract class CharacterGroundedState : CharacterMotionState
    {
        public override bool ApplyGravity => false;
        public override bool SnapToGround => true;
        public override float StepCycleLength => m_StepLength;

        [Tooltip("How much distance does this character need to cover to be considered a step.")]
        [SerializeField, Range(0.1f, 10f)]
        protected float m_StepLength = 1.2f;

        [Tooltip("The forward speed of this character.")]
        [SerializeField, Range(0.1f, 10f)]
        protected float m_ForwardSpeed = 2.5f;

        [Tooltip("The backward speed of this character.")]
        [SerializeField, Range(0.1f, 10f)]
        protected float m_BackSpeed = 2.5f;

        [Tooltip("The sideway speed of this character.")]
        [SerializeField, Range(0.1f, 10f)]
        protected float m_SideSpeed = 2.5f;


        public override bool IsStateValid() => Motor.IsGrounded;

        public override Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime)
        {
            Vector3 targetVelocity = GetTargetVelocity(Input.MovementInput, currentVelocity);

            float targetAcceleration;

            // Calculate the rate at which the current speed should increase / decrease. 
            if (targetVelocity.sqrMagnitude > 0.001f)
            {
                // Get the velocity mod.
                float velocityMod = Controller.VelocityModifier.Get() * Motor.GetSlopeSpeedMultiplier();

                // Finally multiply the target velocity with the velocity modifier.
                targetVelocity *= velocityMod;

                targetAcceleration = Controller.AccelerationModifier.Get();
            }
            else
                targetAcceleration = Controller.DecelerationModifier.Get();

            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, targetAcceleration * deltaTime);

            return currentVelocity;
        }

        protected virtual Vector3 GetTargetVelocity(Vector3 moveDirection, Vector3 currentVelocity)
        {
            bool wantsToMove = moveDirection.sqrMagnitude > 0f;
            moveDirection = (wantsToMove ? moveDirection : currentVelocity.normalized);

            float desiredSpeed = 0f;

            if (wantsToMove)
            {
                // Set the default speed (forward)
                desiredSpeed = m_ForwardSpeed;

                // Sideways movement
                if (Mathf.Abs(Input.RawMovementInput.x) > 0.01f)
                    desiredSpeed = m_SideSpeed;

                // Back movement
                if (Input.RawMovementInput.y < 0f)
                    desiredSpeed = m_BackSpeed;
            }

            return moveDirection * desiredSpeed;
        }
    }
}