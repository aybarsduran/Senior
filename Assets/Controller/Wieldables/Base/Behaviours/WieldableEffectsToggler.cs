using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Utilities/Effects Toggler")]
    public sealed class WieldableEffectsToggler : MonoBehaviour
    {
        [SpaceArea]
        [SerializeField, ReorderableList(HasLabels = false)]
        private LightEffect[] m_LightEffects;

        [SerializeField, ReorderableList(HasLabels = false)]
        private AudioEffect[] m_AudioEffects;

        [SerializeField, ReorderableList(HasLabels = false)]
        private ParticleSystem[] m_Particles;


        public void EnableEffects() 
        {
            for (int i = 0; i < m_LightEffects.Length; i++)
                m_LightEffects[i].Play(true);

            for (int i = 0; i < m_AudioEffects.Length; i++)
                m_AudioEffects[i].Play();

            for (int i = 0; i < m_Particles.Length; i++)
                m_Particles[i].Play(true);
        }

        public void DisableEffects() 
        {
            for (int i = 0; i < m_LightEffects.Length; i++)
                m_LightEffects[i].Stop(true);

            for (int i = 0; i < m_AudioEffects.Length; i++)
                m_AudioEffects[i].Stop();

            for (int i = 0; i < m_Particles.Length; i++)
                m_Particles[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}