using IdenticalStudios.ProceduralMotion;
using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
   
    public sealed class JumpMotionModule : DataMotionModule<CurvesMotionData>
    {
        private float m_CurrentJumpTime;
        private bool m_PlayJumpAnim;
        private float m_RandomFactor = -1f;

        private const float k_PositionForceMod = 0.05f;
        private const float k_RotationForceMod = 5f;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule<IMovementController>().AddStateEnterListener(MovementStateType.Jump, OnJump);
        }

        protected override void OnBehaviourDisabled()
        {
            base.OnBehaviourDisabled();
            GetModule<IMovementController>().RemoveStateEnterListener(MovementStateType.Jump, OnJump);
        }

        protected override CurvesMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            if (dataHandler.TryGetData<GeneralMotionData>(out var data))
                return data.Jump;

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
            if (Data == null || !m_PlayJumpAnim)
                return;

            bool playPosJump = Data.PositionCurves.Duration > m_CurrentJumpTime;
            if (playPosJump)
            {
                // Evaluate position jumping curves.
                Vector3 posJump = Data.PositionCurves.Evaluate(m_CurrentJumpTime);
                posJump = motionMixer.TargetTransform.InverseTransformVector(posJump);

                posJump = new(k_PositionForceMod * posJump.x * m_RandomFactor,
                    posJump.y * k_PositionForceMod,
                    posJump.z * k_PositionForceMod);

                SetTargetRotation(posJump);
            }

            bool playRotJump = Data.RotationCurves.Duration > m_CurrentJumpTime;
            if (playRotJump)
            {
                // Evaluate rotation jumping curves.
                Vector3 rotJump = Data.RotationCurves.Evaluate(m_CurrentJumpTime);

                rotJump = new(rotJump.x * k_RotationForceMod,
                    rotJump.y * k_RotationForceMod * m_RandomFactor,
                    rotJump.z * k_RotationForceMod * m_RandomFactor);

                SetTargetRotation(rotJump);
            }
            m_CurrentJumpTime += deltaTime;

            if (!playPosJump && !playRotJump)
            {
                m_PlayJumpAnim = false;
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }
        }

        private void OnJump()
        {
            m_CurrentJumpTime = 0f;
            m_PlayJumpAnim = true;
            m_RandomFactor = m_RandomFactor > 0f ? -1f : 1f;

            TickMotionLogic(Time.deltaTime);
        }
    }
}
