using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class StrafeMotionModule : DataMotionModule<SwayMotionData>
    {
        private ICharacterMotor m_Motor;

        private const float k_PositionForceMod = 0.01f;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule(out m_Motor);
        }

        protected override SwayMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            if (dataHandler.TryGetData<GeneralMotionData>(out var data))
                return data.Strafe;

            return null;
        }

        protected override void OnDataChanged(SwayMotionData data)
        {
            if (data != null)
            {
                positionSpring.Adjust(data.PositionSettings);
                rotationSpring.Adjust(data.RotationSettings);
            }
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (Data == null || (positionSpring.IsIdle && rotationSpring.IsIdle && m_Motor.SimulatedVelocity == Vector3.zero))
                return;

            // Calculate the strafe input.
            Vector3 strafeInput = transform.InverseTransformVector(m_Motor.SimulatedVelocity);
            strafeInput = Vector3.ClampMagnitude(strafeInput, Data.MaxSwayLength);

            // Calculate the strafe position sway.
            Vector3 posSway = new()
            {
                x = strafeInput.x * Data.PositionSway.x * k_PositionForceMod,
                y = -Mathf.Abs(strafeInput.x * Data.PositionSway.y) * k_PositionForceMod,
                z = -strafeInput.z * Data.PositionSway.z * k_PositionForceMod
            };

            // Calculate the strafe rotation sway.
            Vector3 rotSway = new()
            {
                x = strafeInput.z * Data.RotationSway.x,
                y = -strafeInput.x * Data.RotationSway.y,
                z = strafeInput.x * Data.RotationSway.z
            };

            SetTargetPosition(posSway);
            SetTargetRotation(rotSway);
        }
    }
}