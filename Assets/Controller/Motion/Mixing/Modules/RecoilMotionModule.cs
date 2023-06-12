using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    public sealed class RecoilMotionModule : MotionModule
    {
        private Vector3 m_TargetRotation;
        private Vector3 m_StartRotation;
        private float m_Interpolation = 1f;
        private float m_InverseDuration;
        private bool m_PlayRecoil;
        
        
        public void AdjustSprings(SpringSettings? rotSettings)
        {
            if (rotSettings != null)
                rotationSpring.Adjust(rotSettings.Value);
        }

        public void AddRecoil(Vector3 recoil, float duration)
        {
            m_StartRotation = Vector3.LerpUnclamped(m_StartRotation, m_TargetRotation, m_Interpolation);
            m_TargetRotation = recoil;

            m_Interpolation = 0f;
            m_InverseDuration = 1f / Mathf.Max(duration, 0.001f);

            m_PlayRecoil = true;
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (!m_PlayRecoil)
                return;

            if (m_Interpolation >= 1f)
            {
                SetTargetRotation(Vector3.zero);
                m_PlayRecoil = false;
                return;
            }

            m_Interpolation = Mathf.Min(m_Interpolation + (deltaTime * m_InverseDuration), 1f);

            Vector3 rotValue = Vector3.SlerpUnclamped(m_StartRotation, m_TargetRotation, m_Interpolation);
            SetTargetRotation(rotValue);
        }
    }
}
