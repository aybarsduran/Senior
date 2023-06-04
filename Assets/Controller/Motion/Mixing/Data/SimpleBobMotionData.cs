using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    public class SimpleBobMotionData : BobMotionDataBase
    {
        public override BobMode BobType => m_BobType;
        public override float BobSpeed => m_BobSpeed;

        public override SpringSettings PositionSettings => SpringSettings.Default;
        public override SpringSettings RotationSettings => SpringSettings.Default;

        public override SpringForce3D PositionStepForce => SpringForce3D.Default;
        public override SpringForce3D RotationStepForce => SpringForce3D.Default;

        public override Vector3 PositionAmplitude => m_PositionAmplitude;
        public override Vector3 RotationAmplitude => m_RotationAmplitude;

        [SerializeField]
        protected BobMode m_BobType = BobMode.StepCycleBased;

        [SerializeField, Range(0.01f, 10f)]
        private float m_BobSpeed = 1f;

        [SerializeField]
        private Vector3 m_PositionAmplitude = Vector3.zero;

        [SerializeField]
        private Vector3 m_RotationAmplitude = Vector3.zero;
    }
}
