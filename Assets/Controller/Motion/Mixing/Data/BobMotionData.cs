using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public class BobMotionData : BobMotionDataBase
    {
        public override BobMode BobType => m_BobType;
        public override float BobSpeed => m_BobSpeed;

        public override SpringSettings PositionSettings => m_PositionSpring;
        public override SpringSettings RotationSettings => m_RotationSpring;

        public override SpringForce3D PositionStepForce => m_PositionStepForce;
        public override SpringForce3D RotationStepForce => m_RotationStepForce;

        public override Vector3 PositionAmplitude => m_PositionAmplitude;
        public override Vector3 RotationAmplitude => m_RotationAmplitude;


        [SerializeField]
        protected BobMode m_BobType = BobMode.StepCycleBased;

        [SerializeField, Range(0.01f, 10f)]
        private float m_BobSpeed = 1f;

        //Position Settings
        [SerializeField]
        private SpringSettings m_PositionSpring;

        [SerializeField]
        private SpringForce3D m_PositionStepForce;

        [SerializeField]
        private Vector3 m_PositionAmplitude = Vector3.zero;

        //Rotation Settings

        [SerializeField]
        private SpringSettings m_RotationSpring;

        [SerializeField]
        private SpringForce3D m_RotationStepForce;

        [SerializeField]
        private Vector3 m_RotationAmplitude = Vector3.zero;
    }
}