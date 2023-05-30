using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    public sealed class AnimCurve1D
    {
        public float Duration => m_Curve[m_Curve.length - 1].time;

        [SerializeField, Range(-10f, 10f)]
        private float m_Multiplier = 1f;

        [SerializeField]
        private AnimationCurve m_Curve = GetDefaultCurve();


        public float Evaluate(float time) => m_Curve.Evaluate(time) * m_Multiplier;
        private static AnimationCurve GetDefaultCurve() => new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(1f, 1f) });
    }
}
