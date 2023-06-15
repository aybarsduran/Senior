using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IdenticalStudios/Motion/View Offset Motion")]
    public sealed class ViewOffsetMotionModule : MotionModule
    {
        [Title("Interpolation")]

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [Title("Rotation Settings")]

        [SerializeField]
        private Vector3 m_PositionOffset;

        [SerializeField]
        private Vector3 m_RotationOffset;

        private ILookHandler m_LookHandler;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule(out m_LookHandler);
        }

        protected override SpringSettings GetDefaultPositionSpringSettings() => m_PositionSpring;
        protected override SpringSettings GetDefaultRotationSpringSettings() => m_RotationSpring;

        public override void TickMotionLogic(float deltaTime)
        {
            float angle = m_LookHandler.ViewAngle.x;
            bool isValidAngle = angle > 30f;

            if (!isValidAngle && positionSpring.IsIdle && rotationSpring.IsIdle)
                return;

            if (isValidAngle)
            {
                float angleFactor = 1f - Mathf.Min(70f - Mathf.Abs(angle), 70f) / 40f;
                Vector3 targetPosition = 0.01f * angleFactor * m_PositionOffset;
                Vector3 targetRotation = m_RotationOffset * angleFactor;

                SetTargetPosition(targetPosition);
                SetTargetRotation(targetRotation);
            }
            else
            {
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            positionSpring.Adjust(m_PositionSpring);
            rotationSpring.Adjust(m_RotationSpring);
        }
#endif
    }
}