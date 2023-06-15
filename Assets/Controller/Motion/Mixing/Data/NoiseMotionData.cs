using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class NoiseMotionData : MotionData
	{
		public float NoiseSpeed => m_NoiseSpeed;
        public float NoiseJitter => m_NoiseJitter;

        public Vector3 PositionAmplitude => m_PositionAmplitude;
        public Vector3 RotationAmplitude => m_RotationAmplitude;

        [Title("General Settings")]

        [SerializeField, Range(0f, 5f)]
        private float m_NoiseSpeed = 1f;

        [SerializeField, Range(0f, 1f)]
        private float m_NoiseJitter = 0f;

        [Title("Position Settings")]

        [SerializeField]
        private Vector3 m_PositionAmplitude = Vector3.zero;

        [Title("Rotation Settings")]

        [SerializeField]
        private Vector3 m_RotationAmplitude = Vector3.zero;
    }
}