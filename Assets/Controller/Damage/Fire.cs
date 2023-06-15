using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    public class Fire : MonoBehaviour
    {
        [SerializeField]
        private LightEffect m_LightEffect;

        [SerializeField]
        private ParticleSystem m_FireParticles;

        [SerializeField]
        private AudioSource m_AudioSource;

        [Space]

        [SerializeField]
        private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        [SerializeField, Range(0f, 1000f)]
        private float m_DamagePerTick = 10f;

        [SerializeField, Range(0.01f, 1000f)]
        private float m_Duration = 5f;

        [SerializeField, Range(0.01f, 5f)]
        private float m_TickDuration = 0.25f;

        [SerializeField, Range(0f, 10f)]
        private float m_StopDuration = 1f;

        private readonly Dictionary<Transform, IDamageReceiver> m_Damageables = new();
        private ICharacter m_Detonator;
        private bool m_IsActive = true;
        private float m_DamageMod = 1f;


        public void Detonate() => Detonate(null);

        public void Detonate(ICharacter character)
        {
            transform.parent = null;
            m_Detonator = character;

            StartCoroutine(C_StartFire());
            StartCoroutine(C_DealFireDamage());
        }

        private void OnTriggerEnter(Collider col)
        {
            if (m_LayerMask == (m_LayerMask | (1 << col.gameObject.layer)))
            {
                IDamageReceiver damageable = col.GetComponent<IDamageReceiver>();

                if (!m_Damageables.ContainsKey(col.transform))
                    m_Damageables.Add(col.transform, damageable);
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (m_LayerMask == (m_LayerMask | (1 << col.gameObject.layer)))
                m_Damageables.Remove(col.transform);
        }

        private IEnumerator C_StartFire()
        {
            m_FireParticles.Stop();
            var main = m_FireParticles.main;

            main.duration = m_Duration;
            m_FireParticles.Play();

            m_LightEffect.Play(true);

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
                transform.SetPositionAndRotation(hit.point, Quaternion.Euler(hit.normal));

            yield return new WaitForSeconds(m_Duration);

            float stopDuration = Time.time + m_StopDuration;

            while (stopDuration > Time.time)
            {
                m_AudioSource.volume -= Time.deltaTime * (1 / m_StopDuration);
                m_DamageMod -= Time.deltaTime * (1 / m_StopDuration);

                yield return null;
            }

            m_FireParticles.Stop();
            m_LightEffect.Stop(true);

            m_IsActive = false;
        }

        private IEnumerator C_DealFireDamage()
        {
            var wait = new WaitForSeconds(m_TickDuration);

            while (m_IsActive)
            {
                foreach (var damageable in m_Damageables)
                {
                    var dmgContext = new DamageContext(DamageType.Fire, transform.position, Vector3.zero, Vector3.zero, m_Detonator);
                    damageable.Value.HandleDamage(m_DamagePerTick * m_DamageMod, dmgContext);
                }

                yield return wait;
            }
        }
    }
}
