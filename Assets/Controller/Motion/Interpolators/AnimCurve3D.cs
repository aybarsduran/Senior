using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public sealed class AnimCurves3D
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

        [SerializeField]
        private AnimationCurve m_ZCurve = GetDefaultCurve();

        private float m_Duration;


        public Vector3 Evaluate(float time)
        {
            return new Vector3()
            {
                x = m_XCurve.Evaluate(time) * m_Multiplier,
                y = m_YCurve.Evaluate(time) * m_Multiplier,
                z = m_ZCurve.Evaluate(time) * m_Multiplier
            };
        }

        public Vector3 Evaluate(float xTime, float yTime, float zTime)
        {
            return new Vector3()
            {
                x = m_XCurve.Evaluate(xTime) * m_Multiplier,
                y = m_YCurve.Evaluate(yTime) * m_Multiplier,
                z = m_ZCurve.Evaluate(zTime) * m_Multiplier
            };
        }

        private float GetDuration()
        {
            float curvesDuration = 0f;

            curvesDuration = GetKeyTimeLargerThan(m_XCurve, curvesDuration);
            curvesDuration = GetKeyTimeLargerThan(m_YCurve, curvesDuration);
            curvesDuration = GetKeyTimeLargerThan(m_ZCurve, curvesDuration);

            return curvesDuration;
        }

        private static float GetKeyTimeLargerThan(AnimationCurve animCurve, float largerThan)
        {
            if (animCurve.length == 0)
                return largerThan;

            float duration = animCurve[animCurve.length - 1].time;
            return duration > largerThan ? duration : largerThan;
        }

        private static AnimationCurve GetDefaultCurve() => new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 0f) });
    }
}