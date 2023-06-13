using System.Collections;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public sealed class StandingTorch : Interactable, ISaveableComponent
    {
        [Title("General (Torch)")]

        [SerializeField, Range(0f, 10f)]
        [Tooltip("Toggle fire on/off cooldown.")]
        private float m_MinToggleTime = 0.5f;

        [SerializeField]
        [Tooltip("Interaction text for when the fire is not ignited.")]
        private string m_EnableFireText = "Ignite Torch";

        [SerializeField]
        [Tooltip("Interaction text for when the fire is ignited.")]
        private string m_DisableFireText = "Extinguish Torch";

        [Title("Effects (Torch)")]

        [SerializeField]
        [Tooltip("Fire particle effect")]
        private ParticleSystem m_FireFX;

        [SerializeField]
        [Tooltip("Fire light effect component.")]
        private LightEffect m_LightEffect;

        [SerializeField]
        [Tooltip("Sound that will be looping while the fire is on.")]
        private AudioClip m_FireLoopSound;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("Looping fire audio vol")]
        private float m_FireAudioVolume = 1f;

        private AudioSource m_AudioSource;

        private bool m_FireEnabled = false;
        private float m_LastToggleTime;

        private Coroutine m_VolumeSetter;


        public override void OnInteract(ICharacter character)
        {
            base.OnInteract(character);

            if (Time.time > m_LastToggleTime + m_MinToggleTime)
            {
                EnableFire(!m_FireEnabled);
                m_LastToggleTime = Time.time;
            }
        }

        public void EnableFire(bool enableFire)
        {
            m_FireEnabled = enableFire;

            if (enableFire)
            {
                m_LightEffect.Play(true);
                m_FireFX.Play(true);
            }
            else
            {
                m_LightEffect.Stop(true);
                m_FireFX.Stop(true);
            }

            Description = enableFire ? m_DisableFireText : m_EnableFireText;

            if (m_VolumeSetter != null)
                StopCoroutine(m_VolumeSetter);

            m_VolumeSetter = StartCoroutine(C_SetAudioVolume(enableFire ? m_FireAudioVolume : 0f));
        }

        private void Awake()
        {
            CreateAudioSource();
            EnableFire(false);
        }

        private void CreateAudioSource()
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1f;
            m_AudioSource.clip = m_FireLoopSound;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0f;
            m_AudioSource.minDistance = 1f;
            m_AudioSource.maxDistance = 10f;

            m_AudioSource.Play();
        }

        private IEnumerator C_SetAudioVolume(float volume)
        {
            while(!Mathf.Approximately(m_AudioSource.volume, volume))
            {
                m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, volume, Time.deltaTime);
                yield return null;
            }
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_FireEnabled = (bool)members[0];

            EnableFire(m_FireEnabled);
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_FireEnabled
            };

            return members;
        }
        #endregion

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            UnityUtils.SafeOnValidate(this, () =>
            {
                if (TryGetComponent(out Buildable buildable))
                {
                    Title = buildable.Definition.Name;
                    Description = buildable.Definition.Description;
                }
            });
        }
#endif
    }
}