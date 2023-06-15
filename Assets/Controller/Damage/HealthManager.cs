using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    /// <summary>
    /// Manages the parent character's health and death
    /// </summary>
    public class HealthManager : MonoBehaviour, IHealthManager, ISaveableComponent
    {
        [System.Serializable]
        public sealed class FloatEvent : UnityEvent<float> { }

        public bool IsAlive => !m_IsDead;

        public float Health
        {
            get => m_CurrentHealth;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, m_MaxHealth);

                if (value != m_CurrentHealth && clampedValue != m_CurrentHealth)
                {
                    m_PrevHealth = m_CurrentHealth;
                    m_CurrentHealth = clampedValue;

                    // Raise respawn event
                    if (m_IsDead && m_CurrentHealth > m_PrevHealth)
                    {
                        m_IsDead = false;
                        m_OnRespawn.Invoke();
                    }
                    // Raise death event
                    else if (!m_IsDead && m_CurrentHealth < 0.01f)
                    {
                        m_IsDead = true;
                        m_OnDeath.Invoke();
                    }
                }
            }
        }

        public float PrevHealth => m_PrevHealth;

        public float MaxHealth
        {
            get => m_MaxHealth;
            set
            {
                float clampedValue = Mathf.Max(value, 0f);

                if (value != m_MaxHealth && clampedValue != m_MaxHealth)
                {
                    m_MaxHealth = clampedValue;
                    MaxHealthChanged?.Invoke(clampedValue);

                    Health = Mathf.Clamp(Health, 0f, m_MaxHealth);
                }
            }
        }

        public event UnityAction<float, DamageContext> DamageTakenFullContext;

        public event UnityAction<float> HealthRestored;
        public event UnityAction<float> MaxHealthChanged;

        public event UnityAction<float> DamageTaken
        {
            add => m_OnDamage.AddListener(value);
            remove => m_OnDamage.RemoveListener(value);
        }

        public event UnityAction Death
        {
            add => m_OnDeath.AddListener(value);
            remove => m_OnDeath.RemoveListener(value);
        }

        public event UnityAction Respawn
        {
            add => m_OnRespawn.AddListener(value);
            remove => m_OnRespawn.RemoveListener(value);
        }

        [SerializeField, Range(0, 1000)]
        [Tooltip("The starting health of this character (can't be higher than the max health).")]
        private float m_StartingHealth = 100f;

        [SerializeField, Range(0, 1000)]
        [Tooltip("The starting max health of this character (can be modified at runtime).")]
        private float m_StartingMaxHealth = 100f;

        [SpaceArea]

        [SerializeField]
        private FloatEvent m_OnDamage;

        [SerializeField]
        private UnityEvent m_OnDeath;

        [SerializeField]
        private UnityEvent m_OnRespawn;

#if UNITY_EDITOR
        [SerializeField, Disable, SpaceArea, NewLabel("Health")]
#endif
        private float m_CurrentHealth;

#if UNITY_EDITOR
        [SerializeField, Disable]
#endif
        private float m_MaxHealth;

        private float m_PrevHealth;
        private bool m_IsDead;


        public void RestoreHealth(float healthRestore)
        {
            float prevHealth = m_CurrentHealth;
            Health += Mathf.Abs(healthRestore);

            if (prevHealth != m_CurrentHealth)
                HealthRestored?.Invoke(healthRestore);
        }

        public void ReceiveDamage(float damage)
        {
            float prevHealth = m_CurrentHealth;

            damage = Mathf.Abs(damage);
            Health -= damage;

            if (prevHealth != m_CurrentHealth)
            {
                m_OnDamage.Invoke(damage);
                DamageTakenFullContext?.Invoke(damage, DamageContext.Default);
            }
        }

        public void ReceiveDamage(float damage, DamageContext dmgContext)
        {
            float prevHealth = m_CurrentHealth;

            damage = Mathf.Abs(damage);
            Health -= damage;

            if (prevHealth != m_CurrentHealth)
            {
                m_OnDamage.Invoke(damage);
                DamageTakenFullContext?.Invoke(damage, dmgContext);
            }
        }

        private void Awake()
        {
            m_MaxHealth = m_StartingMaxHealth;
            m_CurrentHealth = Mathf.Clamp(m_StartingHealth, 0f, m_MaxHealth);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_CurrentHealth = (float)members[0];
            m_MaxHealth = (float)members[1];
            m_IsDead = (bool)members[2];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_CurrentHealth,
                m_MaxHealth,
                m_IsDead
            };

            return members;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void OnValidate()
        {
            MaxHealth = m_StartingMaxHealth;
            Health = m_StartingHealth;
        }
#endif
        #endregion
    }
}