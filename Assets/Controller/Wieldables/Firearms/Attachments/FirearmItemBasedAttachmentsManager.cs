using IdenticalStudios.InventorySystem;
using IdenticalStudios.WieldableSystem.Effects;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    /// <summary>
    /// TODO: Refactor.
    /// </summary>
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Handlers/Item-Based Attachments")]
    [RequireComponent(typeof(IFirearm))]
    public class FirearmItemBasedAttachmentsManager : WieldableItemBehaviour
    {
        #region Internal
        [Serializable]
        public sealed class AttachmentItemConfigurations
        {
            [NonSerialized]
            public UnityAction AttachmentChangedCallback;

            [SerializeField, NewLabel("Property")]
            private DataIdReference<ItemPropertyDefinition> m_AttachmentTypeProperty;

            [SpaceArea]

            [SerializeField, ReorderableList(ListStyle.Lined, childLabel: "m_Attachment")]
            private AttachmentItemConfiguration[] m_Configurations;


            public void AttachToItem(IItem item)
            {
                if (item.TryGetPropertyWithId(m_AttachmentTypeProperty, out var property))
                {
                    AttachConfigurationWithID(property.ItemId);
                    property.Changed += OnPropertyChanged;
                }
            }

            public void DetachFromItem(IItem item)
            {
                if (item.TryGetPropertyWithId(m_AttachmentTypeProperty, out var property))
                    property.Changed -= OnPropertyChanged;
            }

            private void OnPropertyChanged(IItemProperty property)
            {
                AttachConfigurationWithID(property.ItemId);
                AttachmentChangedCallback?.Invoke();
            }

            private void AttachConfigurationWithID(int id)
            {
                foreach (var config in m_Configurations)
                {
                    if (config.CorrespondingItem != id)
                        continue;
                    
                    config.Attach();
                    return;
                }
            }
        }

        [Serializable]
        public class AttachmentItemConfiguration
        {
            public int CorrespondingItem => m_Item;

            [SerializeField]
            private DataIdReference<ItemDefinition> m_Item;

            [SerializeField]
            private FirearmAttachmentBehaviour m_Attachment;


            public void Attach() => m_Attachment.Attach();
        }
        #endregion

        [SerializeField, ReorderableList(childLabel: "Config", Foldable = true)]
        private AttachmentItemConfigurations[] m_Configurations;

        [Title("Effects")]

        [SerializeField]
        private EffectCollection m_SwapEffects;

        private IItem m_PrevItem;


        protected override void OnItemChanged(IItem item)
        {
            if (m_PrevItem != null)
            {
                for (int i = 0; i < m_Configurations.Length; i++)
                    m_Configurations[i].DetachFromItem(m_PrevItem);
            }

            if (item != null)
            {
                m_PrevItem = item;
                for (int i = 0; i < m_Configurations.Length; i++)
                    m_Configurations[i].AttachToItem(item);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (var config in m_Configurations)
                config.AttachmentChangedCallback = OnAttachmentSwapped;
        }

        private void OnAttachmentSwapped() => m_SwapEffects.PlayEffects(Item.Wieldable);
    }
}