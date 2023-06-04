using IdenticalStudios.ProceduralMotion;
using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
   
    public sealed class LookMotionModule : DataMotionModule<SwayMotionData>
    {
        private ILookHandler m_LookHandler;

        private const float k_PositionForceMod = 0.02f;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule(out m_LookHandler);
        }

        protected override SwayMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            if (dataHandler.TryGetData<GeneralMotionData>(out var data))
                return data.Look;

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
            if (Data == null)
                return;

            // Calculate the look input.
            Vector2 lookInput = m_LookHandler.LookInput;
            lookInput = Vector2.ClampMagnitude(lookInput, Data.MaxSwayLength);

            Vector3 posSway = new(
                lookInput.y * Data.PositionSway.x * k_PositionForceMod,
                lookInput.x * Data.PositionSway.y * -k_PositionForceMod);

            Vector3 rotSway = new(
                lookInput.x * Data.RotationSway.x,
                lookInput.y * -Data.RotationSway.y,
                lookInput.y * -Data.RotationSway.z);

            SetTargetPosition(posSway);
            SetTargetRotation(rotSway);
        }
    }
}