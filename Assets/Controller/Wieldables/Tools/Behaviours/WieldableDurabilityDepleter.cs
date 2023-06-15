using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(Wieldable))]
    [AddComponentMenu("IdenticalStudios/Wieldables/Utilities/Durability Depleater")]
    public sealed class WieldableDurabilityDepleter : WieldableItemBehaviour
    {
        [SerializeField, Suffix(" / sec")]
        private float m_Depletion = 1f;
        
        [SerializeField]
        private DataIdReference<ItemPropertyDefinition> m_DurabilityProperty;
        
        [SerializeField]
        private StandardSound m_DurabilityDepletedSound;
        
        [SpaceArea]
        
        [SerializeField]
        private UnityEvent m_OnDurabilityDepleted;

        [SerializeField]
        private UnityEvent m_OnDurabilityRestored;

        private float m_DurabilityToDeplete;
        private bool m_DurabilityDepleted;
        private float m_LastDurability;

        private IItemProperty m_Durability;
        private IWieldable m_Wieldable;


        protected override void OnItemChanged(IItem item)
        {
            if (item != null)
            {
                m_Durability = item.GetPropertyWithId(m_DurabilityProperty);
                m_DurabilityDepleted = m_Durability != null && m_Durability.Float < 0.01f;

                if (m_DurabilityDepleted)
                    m_OnDurabilityDepleted.Invoke();
                else
                    m_OnDurabilityRestored.Invoke();
            }
            else
                m_Durability = null;
        }

        protected override void Awake()
        {
            base.Awake();
            m_Wieldable = GetComponentInParent<IWieldable>();
        }

        private void FixedUpdate()
        {
            if (m_Durability == null)
                return;

            m_DurabilityToDeplete += m_Depletion * Time.fixedDeltaTime;

            if (m_DurabilityToDeplete >= m_Depletion)
            {
                // On Durability Increased && Previoulsy fully depleted
                if (m_DurabilityDepleted && m_Durability.Float > m_LastDurability)
                {
                    m_DurabilityDepleted = false;
                    m_OnDurabilityRestored.Invoke();
                }

                m_Durability.Float = Mathf.Max(m_Durability.Float - m_DurabilityToDeplete, 0f);

                m_LastDurability = m_Durability.Float;
                m_DurabilityToDeplete = 0f;

                // On Depleted Durability
                if (!m_DurabilityDepleted && m_LastDurability <= 0.001f)
                {
                    m_DurabilityDepleted = true;
                    m_OnDurabilityDepleted.Invoke();

                    m_Wieldable.AudioPlayer.PlaySound(m_DurabilityDepletedSound);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityUtils.SafeOnValidate(this, () =>
            {
                if (m_DurabilityProperty.IsNull)
                    m_DurabilityProperty = ItemPropertyDefinition.GetWithName("Durability").Id;
            });
        }
#endif
    }
}