using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class SwayMotionData : MotionDataBase
    {
        public SpringSettings PositionSettings => m_PositionSpring;
        public SpringSettings RotationSettings => m_RotationSpring;
        public float MaxSwayLength => m_MaxSwayLength;
        public Vector3 PositionSway => m_PositionSway;
        public Vector3 RotationSway => m_RotationSway;

        [Title("General Settings")]

        [SerializeField, Range(0f, 100f)]
        private float m_MaxSwayLength = 10f;
        
        [Title("Position Settings")]

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [SerializeField]
        private Vector3 m_PositionSway;

        [Title("Rotation Settings")]

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SerializeField]
        private Vector3 m_RotationSway;
    }
}