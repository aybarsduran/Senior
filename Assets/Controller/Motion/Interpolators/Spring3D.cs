using IdenticalStudios.ProceduralMotion;
using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class Spring3D
    {
        public bool IsIdle;

        public Vector3 Value => m_Value;
        public Vector3 Velocity => m_Velocity;
        public Vector3 Acceleration => m_Acceleration;
        public Vector3 TargetValue => m_TargetValue;
        public SpringSettings SpringSettings => m_Settings;

        private Vector3 m_Value;
        private Vector3 m_Velocity;
        private Vector3 m_Acceleration;
        private Vector3 m_TargetValue;
        private SpringSettings m_Settings;

        private const float k_StepSize = 1f / 61f;
        public Spring3D() : this(SpringSettings.Default) { }

        public Spring3D(SpringSettings settings)
        {
            m_Settings = settings;

            IsIdle = true;
            m_TargetValue = Vector3.zero;
            m_Velocity = Vector3.zero;
            m_Acceleration = Vector3.zero;
        }

        public void Adjust(SpringSettings settings) => m_Settings = settings;

        // Reset all values to initial states.
        public void Reset()
        {
            IsIdle = true;
            m_Value = Vector3.zero;
            m_Velocity = Vector3.zero;
            m_Acceleration = Vector3.zero;
        }

        // Sets the target value in the middle of motion.
        // This reuse the current velocity and interpolate the value smoothly afterwards.
        // <param name="value">Target value</param>
        public void SetTargetValue(Vector3 value)
        {
            m_TargetValue = value;
            IsIdle = false;
        }

        // Sets the target value in the middle of motion but using a new velocity.
        // <param name="value">Target value</param>
        // <param name="velocity">New velocity</param>
        public void SetTargetValue(Vector3 value, Vector3 velocity)
        {
            m_TargetValue = value;
            m_Velocity = velocity;
            IsIdle = false;
        }

        // Advance a step by deltaTime(seconds).
        // <param name="deltaTime">Delta time since previous frame</param>
        // <returns>Evaluated Value</returns>
        public Vector3 Evaluate(float deltaTime)
        {
            if (IsIdle)
                return Vector3.zero;

            float damp = m_Settings.Damping;
            float stf = m_Settings.Stiffness;
            float mass = m_Settings.Mass;

            Vector3 val = m_Value;
            Vector3 vel = m_Velocity;
            Vector3 acc = m_Acceleration;

            float stepSize = deltaTime * m_Settings.Speed;
            float _stepSize = stepSize > k_StepSize ? k_StepSize : stepSize - 0.001f;
            float steps = (int)(stepSize / _stepSize + 0.5f);

            for (var i = 0; i < steps; i++)
            {
                var dt = (i - (steps - 1)).GetAbsoluteValue() < 0.01f ? stepSize - i * _stepSize : _stepSize;

                val += vel * dt + acc * (dt * dt * 0.5f);

                Vector3 _acc = (-stf * (val - m_TargetValue) + (-damp * vel)) / mass;

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