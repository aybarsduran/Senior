using IdenticalStudios;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    /// <summary>
    /// Will register damage events from outside and pass them to a health manager.
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class BasicHitbox : MonoBehaviour, IDamageReceiver
    {
        [System.Serializable]
        public sealed class FloatEvent : UnityEvent<float> { }

        public Collider Collider => m_Collider;
        public Rigidbody Rigidbody => m_Rigidbody;

        [SerializeField, Range(0f, 100f)]
        private float m_DamageMultiplier = 1f;


        [SerializeField]//This event will be raised upon taking damage. Any Health Managers attached to this object will also be notified.
        private FloatEvent m_OnDamage;

        private Collider m_Collider;
        private Rigidbody m_Rigidbody;
        private IHealthManager m_HealthManager;


        public DamageResult HandleDamage(float damage, DamageContext dmgContext)
        {
            if (enabled)
            {
                damage *= m_DamageMultiplier;

                m_OnDamage.Invoke(-(damage * m_DamageMultiplier));

                m_Rigidbody.AddForceAtPosition(dmgContext.HitForce, dmgContext.HitPoint, ForceMode.Impulse);
                m_HealthManager?.ReceiveDamage(damage, dmgContext);

                return DamageResult.Default;
            }

            return DamageResult.Ignored;
        }

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_HealthManager = GetComponentInParent<IHealthManager>();
        }
    }
}