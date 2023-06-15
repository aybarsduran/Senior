using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public sealed class AnimCurves2D
    {
        public float Duration
        {
            get
            {
                if (m_Duration < 0.001f)
                    m_Duration = GetDuration();

                return m_Duration;
            }
        }

        [SerializeField, Range(-10f, 10f)]
        private float m_Multiplier = 1f;

        [Header("Curves")]

        [SerializeField]
        private AnimationCurve m_XCurve = GetDefaultCurve();

        [SerializeField]
        private AnimationCurve m_YCurve = GetDefaultCurve();
        
        private float m_Duration;
        
        
        public Vector2 Evaluate(float time)
        {
            return new Vector2()
            {
                x = m_XCurve.Evaluate(time) * m_Multiplier,
                y = m_YCurve.Evaluate(time) * m_Multiplier
            };
        }

        public Vector2 Evaluate(float xTime, float yTime)
        {
            return new Vector2()
            {
                x = m_XCurve.Evaluate(xTime) * m_Multiplier,
                y = m_YCurve.Evaluate(yTime) * m_Multiplier
            };
        }

        public Vector3 EvaluateVec3(float xTime, float zTime)
        {
            return new Vector3()
            {
                x = m_XCurve.Evaluate(xTime) * m_Multiplier,
                y = 0f,
                z = m_YCurve.Evaluate(zTime) * m_Multiplier
            };
        }

        private float GetDuration()
        {
            float curvesDuration = 0f;

            curvesDuration = GetKeyTimeLargerThan(m_XCurve, curvesDuration);
            curvesDuration = GetKeyTimeLargerThan(m_YCurve, curvesDuration);

            return curvesDuration;
        }

        private static float GetKeyTimeLargerThan(AnimationCurve animCurve, float largerThan)
        {
            foreach (var key in animCurve.keys)
            {
                if (key.time > largerThan)
                    largerThan = key.time;
            }

            return largerThan;
        }

        private static AnimationCurve GetDefaultCurve() => new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(1f, 1f) });
    }
}