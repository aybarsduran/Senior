using IdenticalStudios.InventorySystem;
using IdenticalStudios.WieldableSystem.Effects;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IdenticalStudios.WieldableSystem
{
    /// <summary>
    /// TODO: Refactor.
    /// </summary>
    [RequireComponent(typeof(IFirearm))]
    public class FirearmIndexBasedAttachmentsManager : WieldableItemBehaviour
    {
       
        [Serializable]
        public class AttachmentIndexConfiguration
        {
            public string Name => m_Name;
            public Sprite Icon => m_Icon;

            [SerializeField]
            private string m_Name;

            [SerializeField]
            private Sprite m_Icon;

            [SerializeField]
            private FirearmAttachmentBehaviour m_Attachment;


            public void Attach() => m_Attachment.Attach();
        }
        
        public AttachmentIndexConfiguration[] Configurations => m_Configurations;
        public AttachmentIndexConfiguration CurrentMode => m_Configurations != null && m_Configurations.Length > 0 ? m_Configurations[m_SelectedIndex] : null;

        public event UnityAction ModeChanged;

        [SerializeField, Range(0f, 10f)]
        private float m_ChangeModeCooldown = 0.35f;

        [SerializeField]
        private DataIdReference<ItemPropertyDefinition> m_AttachmentIndexProperty;


        [SerializeField, ReorderableList(childLabel: "Attachment Configuration")]
        private AttachmentIndexConfiguration[] m_Configurations;


        [SerializeField, FormerlySerializedAs("m_SwapEffects")]
        private EffectCollection m_ChangeModeEffects;

        private IItemProperty m_AttachedProperty;
        private float m_NextTimeCanToggle;
        private int m_SelectedIndex;


        protected override void OnItemChanged(IItem item)
        {
            if (m_AttachedProperty != null)
                m_AttachedProperty.Changed -= OnPropertyChanged;

            if (item != null && item.TryGetPropertyWithId(m_AttachmentIndexProperty, out m_AttachedProperty))
            {
                AttachConfigurationWithIndex(m_AttachedProperty.Integer);
                m_AttachedProperty.Changed += OnPropertyChanged;
            }
        }

        public void ToggleNextAttachment()
        {
            if (m_AttachedProperty != null && Time.time < m_NextTimeCanToggle)
                return;

            int lastIndex = m_AttachedProperty.Integer;
            m_AttachedProperty.Integer = (int)Mathf.Repeat(m_AttachedProperty.Integer + 1, (m_Configurations.Length - 1) + 0.01f);

            if (m_AttachedProperty.Integer != lastIndex)
            {
                m_ChangeModeEffects.PlayEffects(Item.Wieldable);
                ModeChanged?.Invoke();
            }

            m_NextTimeCanToggle = Time.time + m_ChangeModeCooldown;
        }

        private void AttachConfigurationWithIndex(int index)
        {
            for (int i = 0; i < m_Configurations.Length; i++)
            {
                if (i == index || i == m_Configurations.Length - 1)
                {
                    m_SelectedIndex = i;
                    m_Configurations[m_SelectedIndex].Attach();
                    return;
                }
            }
        }

        private void OnPropertyChanged(IItemProperty property) => AttachConfigurationWithIndex(property.Integer);

        private void OnValidate()
        {
            if (m_AttachmentIndexProperty.IsNull)
            {
                UnityUtils.SafeOnValidate(this, () =>
                {
                    if (ItemPropertyDefinition.TryGetWithName("Fire Mode", out var def))
                        m_AttachmentIndexProperty = def.Id;
                });
            }
        }
    }
}