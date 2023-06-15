using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IdenticalStudios/Motion/Land Motion")]
    public sealed class LandMotionModule : DataMotionModule<CurvesMotionData>
    {
        [Title("Settings")]

        [SerializeField, Range(1f, 100f)]
        private float m_MinLandSpeed = 3f;

        [SerializeField, Range(0f, 100f)]
        private float m_MaxLandSpeed = 15f;

        private float m_CurrentFallTime;
        private float m_LandSpeedFactor;

        private const float k_PositionForceMod = 0.02f;
        private const float k_RotationForceMod = 5f;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule<ICharacterMotor>().FallImpact += OnFallImpact;
        }

        protected override void OnBehaviourDisabled()
        {
            base.OnBehaviourDisabled();
            GetModule<ICharacterMotor>().FallImpact -= OnFallImpact;
        }

        protected override CurvesMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            if (dataHandler.TryGetData<GeneralMotionData>(out var data))
                return data.Land;
            
            return null;
        }

        protected override void OnDataChanged(CurvesMotionData data)
        {
            if (data != null)
            {
                positionSpring.Adjust(data.PositionSettings);
                rotationSpring.Adjust(data.RotationSettings);
            }
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (Data == null || m_LandSpeedFactor <= 0f)
                return;

            bool playPosLand = Data.PositionCurves.Duration > m_CurrentFallTime;
            if (playPosLand)
            {
                // Evaluate position landing curves.
                Vector3 posLand = Data.PositionCurves.Evaluate(m_CurrentFallTime) * m_LandSpeedFactor;
                posLand = motionMixer.TargetTransform.InverseTransformVector(posLand);
                SetTargetPosition(posLand, k_PositionForceMod);
            }

            bool playRotLand = Data.RotationCurves.Duration > m_CurrentFallTime;
            if (playRotLand)
            {
                // Evaluate rotation landing curves.
                Vector3 rotLand = Data.RotationCurves.Evaluate(m_CurrentFallTime) * m_LandSpeedFactor;
                SetTargetRotation(rotLand, k_RotationForceMod);
            }

            m_CurrentFallTime += deltaTime;

            if (!playPosLand && !playRotLand)
            {
                m_LandSpeedFactor = -1f;
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }
        }

        private void OnFallImpact(float landSpeed)
        {
            float impactVelocityAbs = Mathf.Abs(landSpeed);

            if (impactVelocityAbs > m_MinLandSpeed)
            {
                m_CurrentFallTime = 0f;
                m_LandSpeedFactor = Mathf.Clamp01(impactVelocityAbs / m_MaxLandSpeed);
            }
        }
    }
}