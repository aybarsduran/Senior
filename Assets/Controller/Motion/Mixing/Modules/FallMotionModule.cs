using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    
    public sealed class FallMotionModule : DataMotionModule<SingleValueMotionData>
    {
        private ICharacterMotor m_Motor;

        private const float k_PositionForceMod = 0.02f;
        private const float k_RotationForceMod = 2f;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule(out m_Motor);
        }

        protected override SingleValueMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            if (dataHandler.TryGetData<GeneralMotionData>(out var data))
                return data.Fall;

            return null;
        }

        protected override void OnDataChanged(SingleValueMotionData data)
        {
            if (data != null)
            {
                positionSpring.Adjust(data.PositionSettings);
                rotationSpring.Adjust(data.RotationSettings);
            }
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (Data == null || (m_Motor.IsGrounded && rotationSpring.IsIdle && positionSpring.IsIdle))
                return;

            float factor = m_Motor.Velocity.y;
            var posFall = new Vector3(0f, factor * Data.PositionValue * k_PositionForceMod, 0f);
            var rotFall = new Vector3(factor * Data.RotationValue * k_RotationForceMod, 0f, 0f);

            SetTargetPosition(posFall);
            SetTargetRotation(rotFall);
        }
    }
}