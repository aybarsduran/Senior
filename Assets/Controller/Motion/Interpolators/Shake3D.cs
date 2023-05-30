using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class Shake3D
    {
        public bool IsDone => Time.time > m_EndTime;

        private readonly float m_XAmplitude;
        private readonly float m_YAmplitude;
        private readonly float m_ZAmplitude;
        private readonly float m_Speed;
        private readonly float m_Duration;
        private readonly float m_EndTime;

        private static readonly AnimationCurve s_DecayCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);


        public Shake3D(ShakeSettings3D settings, float scale = 1f)
        {
            float xSign = Random.Range(0, 100) > 50 ? 1f : -1f;
            float ySign = Random.Range(0, 100) > 50 ? 1f : -1f;
            float zSign = Random.Range(0, 100) > 50 ? 1f : -1f;

            m_XAmplitude = xSign * scale * settings.XAmplitude;
            m_YAmplitude = ySign * scale * settings.YAmplitude;
            m_ZAmplitude = zSign * scale * settings.ZAmplitude;

            m_Duration = settings.Duration;
            m_Speed = settings.Speed;
            m_EndTime = Time.time + m_Duration;
        }

        public Vector3 Evaluate()
        {
            float time = Time.time;
            float timer = (m_EndTime - time) * m_Speed;
            float decay = s_DecayCurve.Evaluate(1f - (m_EndTime - time) / m_Duration);

            return new Vector3(Mathf.Sin(timer) * m_XAmplitude * decay,
                Mathf.Cos(timer) * m_YAmplitude * decay,
                Mathf.Sin(timer) * m_ZAmplitude * decay);
        }
    }
}