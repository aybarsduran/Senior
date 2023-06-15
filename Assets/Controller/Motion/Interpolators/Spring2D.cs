using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class Spring2D
    {
#if UNITY_EDITOR
        public bool IsIdle { get; private set; }
#else
        public bool IsIdle;
#endif

        public Vector2 Value => m_Value;
        public Vector2 Velocity => m_Velocity;
        public Vector2 Acceleration => m_Acceleration;
        public Vector2 TargetValue => m_TargetValue;
        public SpringSettings SpringSettings => m_Settings;

        private Vector2 m_Value;
        private Vector2 m_Velocity;
        private Vector2 m_Acceleration;
        private Vector2 m_TargetValue;
        private SpringSettings m_Settings;


        public Spring2D() : this(SpringSettings.Default) { }

        public Spring2D(SpringSettings settings) 
        {
            IsIdle = true;
            m_TargetValue = Vector2.zero;
            m_Velocity = Vector2.zero;
            m_Acceleration = Vector2.zero;
            m_Settings = settings;
        }

        public void Adjust(SpringSettings settings) => m_Settings = settings;

        /// <summary>
        /// Reset all values to initial states.
        /// </summary>
        public void Reset()
        {
            IsIdle = true;
            m_Value = Vector2.zero;
            m_Velocity = Vector2.zero;
            m_Acceleration = Vector2.zero;
        }

        /// <summary>
        /// Sets the target value in the middle of motion.
        /// This reuse the current velocity and interpolate the value smoothly afterwards.
        /// </summary>
        /// <param name="value">Target value</param>
        public void SetTargetValue(Vector2 value)
        {
            SetTargetValue(value, m_Velocity);
            IsIdle = false;
        }

        /// <summary>
        /// Sets the target value in the middle of motion but using a new velocity.
        /// </summary>
        /// <param name="value">Target value</param>
        /// <param name="velocity">New velocity</param>
        public void SetTargetValue(Vector2 value, Vector2 velocity)
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
        public Vector2 Evaluate(float deltaTime)
        {
            if (IsIdle)
                return Vector2.zero;

            float damp = m_Settings.Damping;
            float stf = m_Settings.Stiffness;
            float mass = m_Settings.Mass;

            Vector2 val = m_Value;
            Vector2 vel = m_Velocity;
            Vector2 acc = m_Acceleration;

            const float maxStepSize = 1f / 61f;
            float stepSize = deltaTime * m_Settings.Speed;
            float _stepSize = stepSize > maxStepSize ? maxStepSize : stepSize - 0.001f;
            float steps = (int)(stepSize / _stepSize + 0.5f);

            for (var i = 0; i < steps; i++)
            {
                var dt = (i - (steps - 1)).GetAbsoluteValue() < 0.001f ? stepSize - i * _stepSize : _stepSize;

                val += vel * dt + acc * (dt * dt * 0.5f);

                Vector2 _acc = ((-stf * (val - m_TargetValue)) + (-damp * vel)) / mass;

                vel += (acc + _acc) * (dt * 0.5f);
                acc = _acc;
            }

            m_Value = val;
            m_Velocity = vel;
            m_Acceleration = acc;

            if (Mathf.Approximately(acc.sqrMagnitude, 0f))
                IsIdle = true;

            return m_Value;
        }
    }
}
