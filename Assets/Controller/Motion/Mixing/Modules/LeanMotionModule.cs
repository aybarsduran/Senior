using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AdditiveForceMotionModule))]
    [AddComponentMenu("IdenticalStudios/Motion/Lean Motion")]
    public sealed class LeanMotionModule : MotionModule
    {
        public float LeanAngle => m_LeanAngle;
        public float LeanSideOffset => m_LeanSideOffset;
        public float LeanHeightOffset => m_LeanHeightOffset;

        public float MaxLeanPercent
        {
            get => m_MaxLeanPercent;
            set
            {
                var leanPercent = Mathf.Clamp01(value);
                m_MaxLeanPercent = leanPercent;
            }
        }

        [SerializeField]
        private AdditiveForceMotionModule m_ForceModule;

        [Title("Interpolation")]

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [Title("Forces")]

        [SerializeField]
        private SpringForce3D m_PositionEnterForce = SpringForce3D.Default;

        [SerializeField]
        private SpringForce3D m_RotationEnterForce = SpringForce3D.Default;

        [Title("Offsets")]

        [SerializeField, Range(-90, 90f)]
        private float m_LeanAngle = 35f;

        [SerializeField, Range(-5f, 5f)]
        private float m_LeanSideOffset = 1f;

        [SerializeField, Range(-5f, 5f)]
        private float m_LeanHeightOffset = 1f;

        private BodyLeanState m_LeanState;
        private float m_MaxLeanPercent = 1f;


        public void SetLeanState(BodyLeanState leanState)
        {
            float posFactor = m_MaxLeanPercent * 0.02f;
            m_ForceModule.AddPositionForce(m_PositionEnterForce, posFactor, SpringType.FastSpring);

            float rotFactor = (leanState == BodyLeanState.Center ? -1 : 1f) * m_MaxLeanPercent;
            m_ForceModule.AddRotationForce(m_RotationEnterForce, rotFactor, SpringType.FastSpring);

            m_LeanState = leanState;
            if (m_LeanState == BodyLeanState.Center)
            {
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }
        }

        protected override SpringSettings GetDefaultPositionSpringSettings() => m_PositionSpring;
        protected override SpringSettings GetDefaultRotationSpringSettings() => m_RotationSpring;

        public override void TickMotionLogic(float deltaTime)
        {
            if (m_LeanState == BodyLeanState.Center)
                return;

            Vector3 targetPos = default;
            Vector3 targetRot = default;

            switch (m_LeanState)
            {
                case BodyLeanState.Left:
                    targetPos = new Vector3(-m_LeanSideOffset * m_MaxLeanPercent, -m_LeanHeightOffset * m_MaxLeanPercent, 0f);
                    targetRot = new Vector3(0f, 0f, m_LeanAngle * m_MaxLeanPercent);
                    break;
                case BodyLeanState.Right:
                    targetPos = new Vector3(m_LeanSideOffset * m_MaxLeanPercent, -m_LeanHeightOffset * m_MaxLeanPercent, 0f);
                    targetRot = new Vector3(0f, 0f, -m_LeanAngle * m_MaxLeanPercent);
                    break;
            }

            SetTargetPosition(targetPos);
            SetTargetRotation(targetRot);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            positionSpring.Adjust(m_PositionSpring);
            rotationSpring.Adjust(m_RotationSpring);

            if (m_ForceModule == null)
                m_ForceModule = GetComponent<AdditiveForceMotionModule>();
        }
#endif
    }
}