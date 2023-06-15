using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField, Range(0f, 10000f)]
        private float m_Force = 105f;

        [SerializeField, Range(0f, 1000f)]
        private float m_Damage = 100f;

        [SerializeField, Range(0f, 100f)]
        private float m_Radius = 15f;

        [SerializeField]
        private LayerMask m_Layers = Physics.DefaultRaycastLayers;

        [SpaceArea]

        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private ParticleSystem m_ParticleSystem;

        [SerializeField]
        private ShakeSettings3D m_CameraShake;


        public void Detonate() => Detonate(null);

        public void Detonate(ICharacter detonator)
        {
            var cols = Physics.OverlapSphere(transform.position, m_Radius, m_Layers, QueryTriggerInteraction.Collide);

            foreach (var col in cols)
            {
                if (col.TryGetComponent(out IDamageReceiver damageable))
                {
                    var dmgContext = GetDamageForHit(col.transform, detonator, out var damage);
                    damageable.HandleDamage(damage, dmgContext);
                }

                if (col.TryGetComponent<ICharacter>(out var character))
                {
                    if (character.TryGetModule(out ICharacterMotor motor))
                        motor.AddForce((character.transform.position - transform.position) * m_Force, ForceMode.Impulse);
                }
                else if (col.attachedRigidbody != null)
                    col.attachedRigidbody.AddExplosionForce(m_Force, transform.position, 2f, m_Radius, ForceMode.Impulse);
            }

            if (m_AudioSource != null) m_AudioSource.Play();
            if (m_ParticleSystem != null) m_ParticleSystem.Play();

            CameraShakeManagerBase.DoShake(transform.position, m_CameraShake, m_Radius);
        }

        private DamageContext GetDamageForHit(Transform hit, ICharacter detonator, out float damage)
        {
            float distToObject = (transform.position - hit.position).sqrMagnitude;
            float explosionRadiusSqr = m_Radius * m_Radius;

            float distanceFactor = 1f - Mathf.Clamp01(distToObject / explosionRadiusSqr);

            damage = m_Damage * distanceFactor;

            return new DamageContext(DamageType.Explosion, hit.position, (hit.position - transform.position).normalized * m_Force, Vector3.zero, detonator);  
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_Radius);
            Gizmos.color = Color.white;
        }
#endif
    }
}