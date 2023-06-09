using UnityEngine;

namespace IdenticalStudios
{
    [RequireComponent(typeof(IHealthManager))]
    public abstract class CharacterVitalModule : MonoBehaviour
    {
        protected enum DepletionType
        {
            IncreaseValue,
            DecreaseValue,
        }

        protected enum ValueThresholdType
        {
            BiggerThan,
            SmallerThan,
            None
        }

        [SerializeField]
        [Range(0f, 1000f)]
        protected float m_InitialValue = 100f;

        [SerializeField]
        [Range(0f, 1000f)]
        protected float m_InitialMaxValue = 100f;


        [SerializeField]
        protected DepletionType m_DepletionType = DepletionType.DecreaseValue;

        [SerializeField]
        [Range(0f, 100f)]
        protected float m_DepletionSpeed = 3f;


        [SerializeField]
        //After the stat value croses this threshold the character will start taking damage.
        protected ValueThresholdType m_DamageThresholdType = ValueThresholdType.SmallerThan;

        [SerializeField]
        [Range(0f, 1000f)]
        protected float m_DamageValueThreshold = 5f;

        [SerializeField]
        [Range(0f, 100f)]
        protected float m_Damage = 3f;


        [SerializeField]
        //After the stat value croses this threshold the character's health will start to restore
        protected ValueThresholdType m_HealThresoldType = ValueThresholdType.BiggerThan;

        [SerializeField]
        [Range(0f, 1000f)]
        protected float m_HealValueThreshold = 95f;

        [SerializeField]
        [Range(0f, 100f)]
        protected float m_HealthRestore = 3f;

        protected IHealthManager m_HealthManager;


        protected virtual void Awake()
        {
            if (!TryGetComponent(out m_HealthManager))
            {
                Debug.LogError($"This component requires a component that implements the 'IVitalsManager' module to function, Disabling... ");
                enabled = false;
            }
        }

        protected void InitalizeStat(ref float statValue, ref float maxStatValue)
        {
            statValue = Mathf.Clamp(m_InitialValue, 0f, m_InitialMaxValue);
            maxStatValue = m_InitialMaxValue;
        }

        protected void DepleteStat(ref float statValue, float maxStatValue, float deltaTime)
        {
            float depletion = (m_DepletionType == DepletionType.IncreaseValue) ? (m_DepletionSpeed * deltaTime) : -(m_DepletionSpeed * deltaTime);

            statValue = Mathf.Clamp(statValue + depletion, 0, maxStatValue);

            // Apply damage
            switch (m_DamageThresholdType)
            {
                case ValueThresholdType.BiggerThan:
                    if (statValue > m_DamageValueThreshold)
                        m_HealthManager.ReceiveDamage(m_Damage * deltaTime);
                    break;
                case ValueThresholdType.SmallerThan:
                    if (statValue < m_DamageValueThreshold)
                        m_HealthManager.ReceiveDamage(m_Damage * deltaTime);
                    break;
                case ValueThresholdType.None:
                    break;
            }

            // Restore health
            switch (m_HealThresoldType)
            {
                case ValueThresholdType.BiggerThan:
                    if (statValue > m_HealValueThreshold)
                        m_HealthManager.RestoreHealth(m_HealthRestore * deltaTime);
                    break;
                case ValueThresholdType.SmallerThan:
                    if (statValue < m_HealValueThreshold)
                        m_HealthManager.RestoreHealth(m_HealthRestore * deltaTime);
                    break;
                case ValueThresholdType.None:
                    break;
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            m_InitialValue = Mathf.Clamp(m_InitialValue, 0f, m_InitialMaxValue);
        }
#endif
    }
}