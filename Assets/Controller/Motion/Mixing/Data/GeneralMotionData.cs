using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    public sealed class GeneralMotionData : MotionData
    {
        public SwayMotionData Look => m_LookData;
        public SwayMotionData Strafe => m_StrafeData;
        public CurvesMotionData Jump => m_JumpData;
        public CurvesMotionData Land => m_LandData;
        public SingleValueMotionData Fall => m_FallData;

        
        [SerializeField]
        private SwayMotionData m_LookData;

        
        [SerializeField]
        private SwayMotionData m_StrafeData;

        [SerializeField]
        private CurvesMotionData m_JumpData;
            
        [SerializeField]
        private CurvesMotionData m_LandData;

        [SerializeField]
        private SingleValueMotionData m_FallData;
    }
}
