using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    public sealed class RetractionMotionData : MotionData
    {
        public float RetractionDistance => m_RetractionDistance;

        public SpringSettings PositionSettings => m_PositionSpring;
        public SpringSettings RotationSettings => m_RotationSpring;

        public Vector3 PositionOffset => m_PositionOffset;
        public Vector3 RotationOffset => m_RotationOffset;

        //General Settings

        [SerializeField, Range(0.1f, 5f)]
        private float m_RetractionDistance = 0.55f;

        //Position Settings

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [SerializeField]
        private Vector3 m_PositionOffset;

        //Rotation Settings

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SerializeField]
        private Vector3 m_RotationOffset;
    }
}