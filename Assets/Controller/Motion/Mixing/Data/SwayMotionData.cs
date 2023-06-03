using IdenticalStudios.ProceduralMotion;
using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public sealed class SwayMotionData : MotionDataBase
    {
        public SpringSettings PositionSettings => m_PositionSpring;
        public SpringSettings RotationSettings => m_RotationSpring;
        public float MaxSwayLength => m_MaxSwayLength;
        public Vector3 PositionSway => m_PositionSway;
        public Vector3 RotationSway => m_RotationSway;

        //General Settings

        [SerializeField, Range(0f, 100f)]
        private float m_MaxSwayLength = 10f;

        //"Position Settings

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [SerializeField]
        private Vector3 m_PositionSway;

        //Rotation Settings

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SerializeField]
        private Vector3 m_RotationSway;
    }
}