using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class Spring1D
    {
#if UNITY_EDITOR
        public bool IsIdle { get; private set; }
#else
        public bool IsIdle;
#endif

        public float Value => m_Value;
        public float Velocity => m_Velocity;
        public float Acceleration => m_Acceleration;
        public float TargetValue => m_TargetValue;
        public SpringSettings SpringSettings => m_Settings;

        private float m_Value;
        private float m_Velocity;
        private float m_Acceleration;
        private float m_TargetValue;
        private SpringSettings m_Settings;


        public Spring1D() : this(SpringSettings.Default) { }

        public Spring1D(SpringSettings settings) 
        {
            m_Settings = settings;

            IsIdle = true;
            m_TargetValue = 0f;
            m_Velocity = 0f;
            m_Acceleration = 0f;
        }

        public void Adjust(SpringSettings settings) => m_Settings = settings;

        /// <summary>
        /// Reset all values to initial states.
        /// </summary>
        public void Reset()
        {
            IsIdle = true;
            m_Value = 0f;
            m_Velocity = 0f;
            m_Acceleration = 0f;
        }

        /// <summary>
        /// Sets the target value in the middle of motion.
        /// This reuse the current velocity and interpolate the value smoothly afterwards.
        /// </summary>
        /// <param name="value">Target value</param>
        public void SetTargetValue(float value)
        {
            m_TargetValue = value;
            IsIdle = false;
        }

        /// <summary>
        /// Sets the target value in the middle of motion but using a new velocity.
        /// </summary>
        /// <param name="value">Target value</param>
        /// <param name="velocity">New velocity</param>
        public void SetTargetValue(float value, float velocity)
        {
            m_TargetValue = value;
            m_Velocity = velocity;
            IsIdle = false;
        }

        /// <summary>
        /// Advance a step by deltaTime(seconds).
        /// </summary>
        /// <param name="deltaTime">Delta time since previous frame</param>
        /// <returns>Evaluated Value</returns>
        public float Evaluate(float deltaTime)
        {
            if (IsIdle)
                return 0f;

            float damp = m_Settings.Damping;
            float stf = m_Settings.Stiffness;
            float mass = m_Settings.Mass;

            float val = m_Value;
            float vel = m_Velocity;
            float acc = m_Acceleration;

            const float maxStepSize = 1f / 61f;
            float stepSize = deltaTime * m_Settings.Speed;
            float _stepSize = stepSize > maxStepSize ? maxStepSize : stepSize - 0.001f;
            float steps = (int)(stepSize / _stepSize + 0.5f);

            for (var i = 0; i < steps; i++)
            {
                var dt = (i - (steps - 1)).GetAbsoluteValue() < 0.001f ? stepSize - i * _stepSize : _stepSize;

                val += vel * dt + acc * (dt * dt * 0.5f);

                float _acc = ((-stf * (val - m_TargetValue)) + (-damp * vel)) / mass;

                vel += (acc + _acc) * (dt * 0.5f);
                acc = _acc;
            }

            m_Value = val;
            m_Velocity = vel;
            m_Acceleration = acc;

            if (Mathf.Approximately(acc, 0f))
                IsIdle = true;

            return m_Value;
        }
    }
}