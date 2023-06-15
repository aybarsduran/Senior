using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class GeneralMotionData : MotionData
    {
        public SwayMotionData Look => m_LookData;
        public SwayMotionData Strafe => m_StrafeData;
        public CurvesMotionData Jump => m_JumpData;
        public CurvesMotionData Land => m_LandData;
        public SingleValueMotionData Fall => m_FallData;

        [SpaceArea(3f)]
        [SerializeField]
        private SwayMotionData m_LookData;

        [SpaceArea(3f)]
        [SerializeField]
        private SwayMotionData m_StrafeData;

        [SpaceArea(3f)]
        [SerializeField]
        private CurvesMotionData m_JumpData;

        [SpaceArea(3f)]
        [SerializeField]
        private CurvesMotionData m_LandData;

        [SpaceArea(3f)]
        [SerializeField]
        private SingleValueMotionData m_FallData;
    }
}
