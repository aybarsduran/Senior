using System.Collections;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [RequireComponent(typeof(CookingStation))]
    public sealed class CampfireEffects : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Wood;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        private ParticleSystem[] m_ParticleEffects;

        [Title("Audio")]

        [SerializeField]
        private AudioClip m_FireLoopSound;

        [SerializeField, Range(0f, 1f)]
        private float m_MinFireVolume = 0.5f;

        [SerializeField]
        private SoundPlayer m_FuelAddAudio;

        [SerializeField]
        private SoundPlayer m_FireExtinguishedAudio;

        [Title("Light")]

        [SerializeField]
        private LightEffect m_LightEffect;

        [SerializeField, Range(0f, 1f)]
        private float m_MinLightIntensity = 0.5f;

        [Title("Material")]

        [SerializeField]
        private Material m_WoodMaterial;

        [SerializeField, Range(0f, 600f)]
        private float m_WoodBurnDuration = 60f;

        private CookingStation m_Cooking;
        private AudioSource m_AudioSource;

        private float m_LastFuelAddTime;
        private int m_BurnedAmountShaderId;


        private void Awake()
        {
            m_Cooking = GetComponent<CookingStation>();

            m_BurnedAmountShaderId = Shader.PropertyToID("_BurnedAmount");

            CreateAudioSource();
            CreateWoodMaterial();
        }

        private void OnEnable()
        {
            m_Cooking.CookingStarted += OnFireStarted;
            m_Cooking.CookingStopped += OnFireStopped;
            m_Cooking.FuelAdded += OnFuelAdded;
        }

        private void OnDisable()
        {
            m_Cooking.CookingStarted -= OnFireStarted;
            m_Cooking.CookingStopped -= OnFireStopped;
            m_Cooking.FuelAdded -= OnFuelAdded;
        }

        private void CreateAudioSource()
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1f;
            m_AudioSource.clip = m_FireLoopSound;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0f;
            m_AudioSource.Play();
        }

        private void CreateWoodMaterial()
        {
            // Create material
            m_WoodMaterial = new Material(m_WoodMaterial);
            m_WoodMaterial.name += "_Instance";
            m_WoodMaterial.SetFloat(m_BurnedAmountShaderId, 0f);

            // Assign it
            var renderers = m_Wood.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in renderers)
                renderer.material = m_WoodMaterial;

            m_Wood.SetActive(false);
        }

        private void OnFireStarted()
        {
            m_Wood.SetActive(true);

            foreach (var effect in m_ParticleEffects)
                effect.Play(false);

            m_LightEffect.Play(true);

            StopAllCoroutines();
            StartCoroutine(C_Update());
        }

        private void OnFireStopped()
        {
            foreach(var effect in m_ParticleEffects)
                effect.Stop(false);

            m_FireExtinguishedAudio.Play2D();
            m_LightEffect.Stop(true);
        }

        private void OnFuelAdded(float fuelDuration)
        {
            m_FuelAddAudio.Play(m_AudioSource);
            m_LastFuelAddTime = Time.time;
        }

        private IEnumerator C_Update()
        {
            while (m_Cooking.CookingActive)
            {
                m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, Mathf.Max(m_Cooking.CookingStrength, m_MinFireVolume), Time.deltaTime * 1f);

                // Shader effects
                float burnedAmount = (Time.time - m_LastFuelAddTime) / m_WoodBurnDuration;
                burnedAmount = Mathf.Clamp01(burnedAmount);
                m_WoodMaterial.SetFloat(m_BurnedAmountShaderId, burnedAmount);

                m_LightEffect.IntensityMultiplier = Mathf.Max(m_MinLightIntensity, m_Cooking.CookingStrength);

                yield return null;
            }

            float reset = 0f;
            while (reset < 1f)
            {
                m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, 0f, 1 - reset);
                m_WoodMaterial.SetFloat(m_BurnedAmountShaderId, 1 - reset);

                reset += Time.deltaTime;

                yield return null;
            }
        }
    }
}