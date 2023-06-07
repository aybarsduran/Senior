using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios
{
    // TODO: Merge with the surface system.
    [RequireComponent(typeof(AudioSource))]
    public sealed class CollisionAudioHandler : MonoBehaviour
    {
        [SerializeField, ReorderableList(ListStyle.Boxed, HasLabels = false)]
        private AudioClip[] m_AudioClips;

        private float m_MaxVolume;
        private AudioSource m_AudioSource;
        private float m_NextTimeCanPlayAudio = 0f;

        private const float k_PlayAudioThreshold = 0.4f;
        private const float k_MinSpeedThreshold = 2f;


        private void OnCollisionEnter(Collision col)
        {
            if (Time.time < m_NextTimeCanPlayAudio)
                return;

            float rVelocity = col.relativeVelocity.magnitude;
            if (rVelocity > k_MinSpeedThreshold)
            {
                float volumeMod = Mathf.Clamp(rVelocity / k_MinSpeedThreshold / 10, 0.2f, 1f);

                m_AudioSource.clip = m_AudioClips.SelectRandom();
                m_AudioSource.volume = m_MaxVolume * volumeMod;
                m_AudioSource.Play();

                m_NextTimeCanPlayAudio = Time.time + k_PlayAudioThreshold;
            }
        }

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_MaxVolume = m_AudioSource.volume;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1f;
            m_AudioSource.playOnAwake = false;
            m_AudioSource.loop = false;
        }
#endif
    }
}