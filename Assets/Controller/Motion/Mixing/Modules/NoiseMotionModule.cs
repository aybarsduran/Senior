using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class NoiseMotionModule : DataMotionModule<NoiseMotionData>
    {
        //Interpolation

        [SerializeField]
        private SpringSettings m_PositionSpring = new(10f, 100f, 1f, 1f);

        [SerializeField]
        private SpringSettings m_RotationSpring = new(10f, 100f, 1f, 1f);

        private const float k_PositionForceMod = 0.01f;
        private const float k_RotationForceMod = 2f;


        protected override NoiseMotionData GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            return dataHandler.GetData<NoiseMotionData>();
        }

        protected override SpringSettings GetDefaultPositionSpringSettings() => m_PositionSpring;
        protected override SpringSettings GetDefaultRotationSpringSettings() => m_RotationSpring;

        public override void TickMotionLogic(float deltaTime)
        {
            if (Data == null)
                return;

            float jitter = Data.NoiseJitter < 0.01f ? 0f : Random.Range(0f, Data.NoiseJitter);
            float speed = Time.time * Data.NoiseSpeed;

            Vector3 posNoise = new()
            {
                x = (Mathf.PerlinNoise(jitter, speed) - 0.5f) * Data.PositionAmplitude.x * k_PositionForceMod,
                y = (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * Data.PositionAmplitude.y * k_PositionForceMod,
                z = (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * Data.PositionAmplitude.z * k_PositionForceMod
            };

            Vector3 rotNoise = new()
            {
                x = (Mathf.PerlinNoise(jitter, speed) - 0.5f) * Data.RotationAmplitude.x * k_RotationForceMod,
                y = (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * Data.RotationAmplitude.y * k_RotationForceMod,
                z = (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * Data.RotationAmplitude.z * k_RotationForceMod
            };

            SetTargetPosition(posNoise);
            SetTargetRotation(rotNoise);
        }
    }
}