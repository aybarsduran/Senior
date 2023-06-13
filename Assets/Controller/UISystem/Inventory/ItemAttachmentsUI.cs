using IdenticalStudios.InventorySystem;
using System;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class ItemAttachmentsUI : MonoBehaviour
    {
        #region Internal
        [Serializable]
        private class PropertySlot
        {
            [NonSerialized]
            public ItemSlotUI SlotUI;

            [NonSerialized]
            public bool Enabled;

            public DataIdReference<ItemPropertyDefinition> Property;
            public Sprite BackgroundIcon;
        }
        #endregion

        [SerializeField, NotNull]
        private ItemSlotUI m_AttachmentTemplate;

        [SerializeField]
        private RectTransform m_AttachmentSlotsRoot;

        [SpaceArea]

        [SerializeField, ReorderableList(childLabel: "BackgroundIcon")]
        private PropertySlot[] m_AttachmentSlots;

        private ItemSlotUI m_Inspected;


        public void UpdatePropertySlots(ItemSlotUI selected)
        {
            m_Inspected = selected;
            bool selectedNotNull = selected != null;
            for (int i = 0; i < m_AttachmentSlots.Length; i++)
            {
                var attachmentSlot = m_AttachmentSlots[i];

                if (selectedNotNull && selected.HasItem && selected.Item.TryGetPropertyWithId(attachmentSlot.Property, out var property))
                {
                    int attachedItemId = property.ItemId;
                    IItem attachedItem = attachedItemId == 0 ? null : new Item(ItemDefinition.GetWithId(attachedItemId));

                    attachmentSlot.SlotUI.ItemSlot.Item = attachedItem;

                    if (!attachmentSlot.Enabled)
                    {
                        attachmentSlot.SlotUI.gameObject.SetActive(true);
                        attachmentSlot.Enabled = true;;

                        attachmentSlot.SlotUI.ItemSlot.ItemChanged += OnSlotChanged;
                    }
                }
                else
                {
                    if (attachmentSlot.Enabled)
                    {
                        attachmentSlot.SlotUI.gameObject.SetActive(false);
                        attachmentSlot.Enabled = false;

                        attachmentSlot.SlotUI.ItemSlot.ItemChanged -= OnSlotChanged;
                    }
                }
            }
        }

        private void OnSlotChanged(ItemSlot.CallbackContext context)
        {
            if (m_Inspected == null || context.Type == ItemSlot.CallbackType.PropertyChanged)
                return;

            var itemSlot = context.Slot;
            var propertySlot = GetPropertySlotWithItemSlot(itemSlot);

            if (m_Inspected.Item.TryGetPropertyWithId(propertySlot.Property, out var itemProperty))
                itemProperty.ItemId = itemSlot.Item != null ? itemSlot.Item.Id : 0;
        }

        private PropertySlot GetPropertySlotWithItemSlot(ItemSlot itemSlot)
        {
            foreach (var slot in m_AttachmentSlots)
            {
                if (slot.SlotUI.ItemSlot == itemSlot)
                    return slot;
            }

            return null;
        }

        private void OnEnable() => ItemSelectorUI.SelectedSlotChanged += UpdatePropertySlots;
        private void OnDisable() => ItemSelectorUI.SelectedSlotChanged -= UpdatePropertySlots;

        private void Awake()
        {
            var root = m_AttachmentSlotsRoot != null ? m_AttachmentSlotsRoot : transform;
            foreach (var attachmentSlot in m_AttachmentSlots)
            {
                ItemSlotUI slot = Instantiate(m_AttachmentTemplate, root);
                if (slot.TryGetInfoOfType<ItemInfoUI>(out var info))
                    info.BGIconImage.sprite = attachmentSlot.BackgroundIcon;

                slot.SetItemSlot(new ItemSlot(null));
                slot.gameObject.SetActive(false);

                attachmentSlot.SlotUI = slot;
            }
        }
    }
}