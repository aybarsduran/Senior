using IdenticalStudios;
using System;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class ParticleSystemCollection : MonoBehaviour
    {
        [SerializeField, ReorderableList(HasLabels = false)]
        private ParticleSystem[] m_Particles = Array.Empty<ParticleSystem>();

        [SerializeField, ReorderableList(HasLabels = false)]
        private LightEffect[] m_LightEffects = Array.Empty<LightEffect>();


        public void Play()
        {
            for (int i = 0; i < m_Particles.Length; i++)
                m_Particles[i].Play(false);

            for (int i = 0; i < m_LightEffects.Length; i++)
                m_LightEffects[i].Play(true);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Particles != null && m_Particles.Length > 0)
                m_Particles = GetComponentsInChildren<ParticleSystem>();

            if (m_LightEffects != null && m_LightEffects.Length > 0)
                m_LightEffects = GetComponentsInChildren<LightEffect>();
        }
#endif
    }
}
