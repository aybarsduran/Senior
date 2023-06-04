using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public sealed class SingleValueMotionData : MotionDataBase
    {
        public SpringSettings PositionSettings => m_PositionSpring;
        public SpringSettings RotationSettings => m_RotationSpring;
        public float PositionValue => m_PositionValue;
        public float RotationValue => m_RotationValue;

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SerializeField, Range(-100f, 100f)]
        private float m_PositionValue;

        [SerializeField, Range(-100f, 100f)]
        private float m_RotationValue;
    }
}
