using IdenticalStudios.Surfaces;
using UnityEngine;

namespace IdenticalStudios
{
    public class FootstepsManager : CharacterBehaviour
    {
        #region Internal
        private enum HumanoidFoot
        {
            Left, Right
        }
        #endregion

        private SurfaceDefinition LastSteppedSurface
        {
            set
            {
                m_LastSurface = value;
                m_HasLastSurface = m_LastSurface != null;
            }
        }

        private HumanoidFoot m_LastFootDown;

        [Title("Raycast Settings")]

        [SerializeField]
        private LayerMask m_GroundMask;

        [SerializeField, Range(0.01f, 1f)]
        private float m_RaycastDistance = 0.3f;

        [SerializeField, Range(0.01f, 0.5f)]
        private float m_RaycastRadius = 0.3f;

        [Title("Footsteps Thresholds")]

        [SerializeField, Range(0f, 24f)]
        private float m_MinVolumeSpeed = 1f;

        [SerializeField, Range(0f, 24f)]
        private float m_MaxVolumeSpeed = 7f;

        [Title("Fall Impact Thresholds")]

        [SerializeField, Range(0f, 25f)]
        [Tooltip("If the impact speed is higher than this threeshold, an effect will be played.")]
        private float m_FallImpactThreshold = 4f;

        [SerializeField, Range(0f, 25f)]
        [Tooltip("If the impact speed is at this threeshold, the fall impact effect audio will be at full effect.")]
        private float m_MaxFallImpactThreshold = 12f;

        private IMovementController m_Movement;
        private ICharacterMotor m_Motor;

        private SurfaceDefinition m_LastSurface;
        private bool m_HasLastSurface;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Movement);
            GetModule(out m_Motor);

            m_Movement.StepCycleEnded += PlayFootstepEffect;
            m_Motor.FallImpact += PlayFallImpactEffects;

            m_Movement.VelocityModifier.Add(GetVelocity);
            m_Movement.AccelerationModifier.Add(GetAcceleration);
            m_Movement.DecelerationModifier.Add(GetDeceleration);
        }

        protected override void OnBehaviourDisabled()
        {
            m_Movement.StepCycleEnded -= PlayFootstepEffect;
            m_Motor.FallImpact -= PlayFallImpactEffects;

            m_Movement.VelocityModifier.Remove(GetVelocity);
            m_Movement.AccelerationModifier.Remove(GetAcceleration);
            m_Movement.DecelerationModifier.Remove(GetDeceleration);
        }

        private float GetVelocity() => m_HasLastSurface ? m_LastSurface.VelocityModifier : 1f;
        private float GetAcceleration() => m_HasLastSurface ? m_LastSurface.SurfaceFriction : 1f;
        private float GetDeceleration() => m_HasLastSurface ? m_LastSurface.SurfaceFriction : 1f;

        private void PlayFootstepEffect()
        {
            MovementStateType stateType = m_Movement.ActiveState;

            if (CheckGround(out RaycastHit hitInfo))
            {
                m_LastFootDown = m_LastFootDown == HumanoidFoot.Left ? HumanoidFoot.Right : HumanoidFoot.Left;
                float volumeMod = Mathf.Clamp(m_Motor.Velocity.magnitude + m_Motor.TurnSpeed, m_MinVolumeSpeed, m_MaxVolumeSpeed) / m_MaxVolumeSpeed;
                LastSteppedSurface = SurfaceManager.SpawnEffect(hitInfo, GetEffectType(stateType), volumeMod);
            }
        }

        private void PlayFallImpactEffects(float impactSpeed)
        {
            if (Mathf.Abs(impactSpeed) > m_FallImpactThreshold)
            {
                if (CheckGround(out RaycastHit hitInfo))
                {
                    float volume = Mathf.Min(1f, impactSpeed / (m_MaxFallImpactThreshold - m_FallImpactThreshold));
                    LastSteppedSurface = SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.FallImpact, volume);
                }
            }
        }

        private bool CheckGround(out RaycastHit hitInfo)
        {
            var ray = new Ray(transform.position + Vector3.up * 0.3f, Vector3.down);

            bool hitSomething = Physics.Raycast(ray, out hitInfo, m_RaycastDistance, m_GroundMask, QueryTriggerInteraction.Ignore);

            if (!hitSomething)
                hitSomething = Physics.SphereCast(ray, m_RaycastRadius, out hitInfo, m_RaycastDistance, m_GroundMask, QueryTriggerInteraction.Ignore);

            return hitSomething;
        }
        
        private static SurfaceEffects GetEffectType(MovementStateType stateType)
        {
            bool isRunning = stateType == MovementStateType.Run;
            SurfaceEffects footstepType = isRunning ? SurfaceEffects.HardFootstep : SurfaceEffects.SoftFootstep;

            return footstepType;
        }
    }
}