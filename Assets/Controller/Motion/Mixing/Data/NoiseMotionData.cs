using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    public sealed class NoiseMotionData : MotionData
    {
        public float NoiseSpeed => m_NoiseSpeed;
        public float NoiseJitter => m_NoiseJitter;

        public Vector3 PositionAmplitude => m_PositionAmplitude;
        public Vector3 RotationAmplitude => m_RotationAmplitude;

        //General Settings

        [SerializeField, Range(0f, 5f)]
        private float m_NoiseSpeed = 1f;

        [SerializeField, Range(0f, 1f)]
        private float m_NoiseJitter = 0f;

        //"Position Settings

        [SerializeField]
        private Vector3 m_PositionAmplitude = Vector3.zero;

        //"Rotation Settings

        [SerializeField]
        private Vector3 m_RotationAmplitude = Vector3.zero;
    }
}