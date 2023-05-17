using IdenticalStudios;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface ICharacterMotor : ICharacterModule
    {
        bool IsGrounded { get; }
        float LastGroundedChangeTime { get; }
        float Gravity { get; }

        Vector3 Velocity { get; }
        Vector3 SimulatedVelocity { get; }
        Vector3 GroundNormal { get; }
        float TurnSpeed { get; }
        float GroundSurfaceAngle { get; }
        CollisionFlags CollisionFlags { get; }

        float DefaultHeight { get; }
        float SlopeLimit { get; }
        float Height { get; }
        float Radius { get; }


        event UnityAction Teleported;
        event UnityAction<bool> GroundedChanged;
        event UnityAction<float> FallImpact;
        event UnityAction<float> HeightChanged;

        bool TrySetHeight(float height);
        bool CanSetHeight(float height);
        void SetHeight(float height);

        bool Raycast(Ray ray, float distance);
        bool Raycast(Ray ray, float distance, out RaycastHit raycastHit);
        bool SphereCast(Ray ray, float distance, float radius);

        float GetSlopeSpeedMultiplier();
        void Teleport(Vector3 position, Quaternion rotation);
        void AddForce(Vector3 force, ForceMode mode, bool snapToGround = false);

        
        // A method that will be called when the character motor needs input. 
        void SetMotionInput(MotionInputCallback motionInput);
    }

   
    // A delegate that will be called when the character motor needs input.
    public delegate Vector3 MotionInputCallback(Vector3 velocity, out bool useGravity, out bool snapToGround);

    public static class CollisionFlagsExtensions
    {
        public static bool Has(this CollisionFlags thisFlags, CollisionFlags flag)
        {
            return (thisFlags & flag) == flag;
        }
    }
}