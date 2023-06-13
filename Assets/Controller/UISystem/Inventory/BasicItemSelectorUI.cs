using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class BasicItemSelectorUI : ItemSelectorUI
    {
        [SerializeField, NotNull]
        private SelectableGroupBaseUI m_ItemSlotsGroup;


        protected override void OnInspectionStarted()
        {
            m_ItemSlotsGroup.SelectedChanged += SetSelectedSlot;
            m_ItemSlotsGroup.HighlightedChanged += SetHighlightedSlot;

            SetSelectedSlot(m_ItemSlotsGroup.Selected);
            RaiseSelectedEvent();
        }

        protected override void OnInspectionEnded()
        {
            m_ItemSlotsGroup.SelectedChanged -= SetSelectedSlot;
            m_ItemSlotsGroup.HighlightedChanged -= SetHighlightedSlot;

            SetSelectedSlot(null);
        }

        private void SetSelectedSlot(SelectableUI selectable)
        {
            var slot = selectable == null ? null : selectable.GetComponent<ItemSlotUI>();

            if (SelectedSlot != null)
                SelectedSlot.ItemSlot.ItemChanged -= OnSlotChanged;

            if (slot != null && slot.HasItemSlot)
            {
                slot.ItemSlot.ItemChanged += OnSlotChanged;
                SelectedSlot = slot;
            }
            else
                SelectedSlot = null;
        }

        private void SetHighlightedSlot(SelectableUI highlighted)
        {
            var slot = highlighted == null ? null : highlighted.GetComponent<ItemSlotUI>();
            HighlightedSlot = slot;
        }

        private void OnSlotChanged(ItemSlot.CallbackContext context) => RaiseSelectedEvent();
    }
}