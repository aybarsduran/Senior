using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    public sealed class RetractionMotionModule : DataMotionModule<RetractionMotionData>
    {
        private IInteractionHandler m_Interaction;

        private const float k_PositionForceMod = 0.02f;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule(out m_Interaction);
        }

        protected override RetractionMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            return dataHandler.GetData<RetractionMotionData>();
        }

        protected override void OnDataChanged(RetractionMotionData data)
        {
            if (data != null)
            {
                positionSpring.Adjust(data.PositionSettings);
                rotationSpring.Adjust(data.RotationSettings);
            }
        }

        public override void TickMotionLogic(float deltaTime)
        {
            bool canRetract = Data != null && m_Interaction.HoveredObjectDistance < Data.RetractionDistance;

            if (!canRetract && positionSpring.IsIdle && rotationSpring.IsIdle)
                return;

            if (canRetract)
            {
                float retractionFactor = (Data.RetractionDistance - m_Interaction.HoveredObjectDistance) / Data.RetractionDistance;

                Vector3 posRetraction = Data.PositionOffset * (retractionFactor * k_PositionForceMod);
                Vector3 rotRetraction = Data.RotationOffset * retractionFactor;

                SetTargetPosition(posRetraction);
                SetTargetRotation(rotRetraction);
            }
            else
            {
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }
        }
    }
}