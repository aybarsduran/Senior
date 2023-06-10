using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class ItemSlotUI : SlotUI<IItem>
    {
        public ItemSlot ItemSlot
        {
            get
            {
#if UNITY_EDITOR
                if (m_ItemSlot == null)
                    Debug.LogError("No item slot is linked to this interface!");
#endif

                return m_ItemSlot;
            }
        }

        public bool HasItem => Item != null;
        public bool HasItemSlot => m_ItemSlot != null;
        public bool HasContainer => HasItemSlot && m_ItemSlot.HasContainer;

        public IItem Item => m_ItemSlot?.Item;
        public IItemContainer Container => m_ItemSlot?.Container;

        private ItemSlot m_ItemSlot;


        public void SetItemSlot(ItemSlot itemSlot)
        {
            if (m_ItemSlot == itemSlot)
                return;

            if (m_ItemSlot != null)
                m_ItemSlot.ItemChanged -= OnSlotChanged;

            m_ItemSlot = itemSlot;

            if (m_ItemSlot != null)
            {
                SetData(m_ItemSlot.Item);
                m_ItemSlot.ItemChanged += OnSlotChanged;
            }
            else
                SetData(null);

            void OnSlotChanged(ItemSlot.CallbackContext context) => SetData(m_ItemSlot.Item);
        }

        public void SetItem(IItem item)
        {
            SetItemSlot(null);
            SetData(item);
        }

        private void OnDestroy() => SetItemSlot(null);
    }
}