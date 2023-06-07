using IdenticalStudios.ProceduralMotion;
using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios
{
    public class BodyLeanHandler : CharacterBehaviour, IBodyLeanHandler
    {
        public BodyLeanState LeanState => m_LeanState;

        [SerializeField, Range(0f, 3f)]
        private float m_LeanCooldown = 0.3f;

        [SerializeField]
        private LayerMask m_ObstructionMask;

        [SerializeField, Range(0f, 1f)]
        private float m_ObstructionPadding = 0.25f;

        [SerializeField, Range(0f, 1f)]
        private float m_MaxLeanObstructionCuttoff = 0.3f;

        [SerializeField, Range(1f, 100f)]
        private float m_MaxAllowedCharacterSpeed = 4f;

        //"References

        [SerializeField]
        private Transform m_ReferenceTransfrom;

        [SerializeField]
        private LeanMotionModule m_BodyLeanMotion;

        [SerializeField]
        private LeanMotionModule m_WieldableLeanMotion;

        //Effects

        [SerializeField]
        private StandardSound m_LeanSound;

        private BodyLeanState m_LeanState = BodyLeanState.Center;
        private ICharacterMotor m_Motor;
        private RaycastHit m_RaycastHit;
        private float m_MaxLeanPercent;
        private float m_NextTimeCanLean;


        public void SetLeanState(BodyLeanState leanState)
        {
            if (Time.time < m_NextTimeCanLean)
                return;

            if (CanLean(leanState))
            {
                SetLeanState_Internal(leanState);
                m_NextTimeCanLean = Time.time + m_LeanCooldown;
            }
        }

        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Motor);
            SetLeanPercent(1f);
        }

        private void Update()
        {
            if (!IsBehaviourEnabled || m_LeanState == BodyLeanState.Center)
                return;

            if (!CanLean(m_LeanState))
                SetLeanState_Internal(BodyLeanState.Center);
        }

        private void SetLeanState_Internal(BodyLeanState leanState)
        {
            if (leanState == m_LeanState)
                return;

            m_LeanState = leanState;
            Character.AudioPlayer.PlaySound(m_LeanSound, Mathf.Max(m_MaxLeanPercent, 0.3f));

            if (leanState == BodyLeanState.Center)
                SetLeanPercent(1f);

            m_BodyLeanMotion.SetLeanState(leanState);
            m_WieldableLeanMotion.SetLeanState(leanState);
        }

        private bool CanLean(BodyLeanState leanState)
        {
            if (leanState == BodyLeanState.Center)
                return true;

            if (m_Motor.Velocity.magnitude > m_MaxAllowedCharacterSpeed || !m_Motor.IsGrounded)
                return false;

            var position = m_ReferenceTransfrom.position;

            var targetPos = new Vector3(leanState == BodyLeanState.Left ? -m_BodyLeanMotion.LeanSideOffset : m_BodyLeanMotion.LeanSideOffset,
                -m_BodyLeanMotion.LeanHeightOffset, 0f);

            targetPos = m_BodyLeanMotion.transform.TransformPoint(targetPos);

            var ray = new Ray(position, targetPos - position);
            float distance = Vector3.Distance(position, targetPos) + m_ObstructionPadding;

            if (PhysicsUtils.SphereCastNonAlloc(ray, 0.2f, distance, out m_RaycastHit, m_ObstructionMask, Character.Colliders))
            {
                // Lower the max lean value.
                SetLeanPercent(m_RaycastHit.distance / distance);
                if (m_MaxLeanPercent < m_MaxLeanObstructionCuttoff)
                    return false;

                return true;
            }

            // Reset the max lean value.
            m_BodyLeanMotion.MaxLeanPercent = 1f;
            m_WieldableLeanMotion.MaxLeanPercent = 1f;
            return true;
        }

        private void SetLeanPercent(float percent)
        {
            m_MaxLeanPercent = percent;
            m_BodyLeanMotion.MaxLeanPercent = percent;
            m_WieldableLeanMotion.MaxLeanPercent = percent;
        }
    }
}
