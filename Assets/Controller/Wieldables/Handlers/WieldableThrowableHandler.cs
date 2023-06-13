using IdenticalStudios.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class WieldableThrowableHandler : CharacterBehaviour, IWieldableThrowableHandler
    {
        #region Internal
        private class ThrowableSlot
        {
            public readonly int ItemId;
            public readonly List<ItemSlot> Slots;


            public ThrowableSlot(int itemId, ItemSlot slot)
            {
                ItemId = itemId;
                Slots.Add(slot);
            }
        }
        #endregion

        public int SelectedIndex
        {
            get => m_SelectedIndex;
            set
            {
                if (m_SelectedIndex != value)
                {
                    m_SelectedIndex = Mathf.Clamp(value, 0, m_ThrowableSlots.Count - 1);
                    ThrowableIndexChanged?.Invoke();
                }
            }
        }

        public event UnityAction ThrowableCountChanged;
        public event UnityAction ThrowableIndexChanged;
        public event UnityAction<Throwable> OnThrow;

        [SerializeField, DataReferenceDetails(NullElementName = "Untagged", HasLabel = false)]
        private DataIdReference<ItemTagDefinition>[] m_ContainerTags;

        private readonly List<IItemContainer> m_Containers = new();
        private readonly List<ThrowableSlot> m_ThrowableSlots = new();

        private IWieldableSelectionHandler m_InventorySelection;
        private IWieldablesController m_Controller;
        private int m_SelectedIndex;


        public bool TryThrow()
        {
            if (!IsBehaviourEnabled || m_Controller.IsEquipping || m_ThrowableSlots.Count == 0 || m_ThrowableSlots[m_SelectedIndex].Slots.Count == 0)
                return false;

            var selectedSlot = m_ThrowableSlots[m_SelectedIndex].Slots[0];
            var throwable = (Throwable)m_InventorySelection.GetWieldableItemWithId(selectedSlot.Item.Id).Wieldable;
            if (m_Controller.TryEquipWieldable(throwable))
            {
                throwable.Use(UsePhase.Start);
                OnThrow?.Invoke(throwable);
                return true;
            }

            return false;
        }

        public void SelectNext(bool next)
        {
            float delta = next ? 1f : -1f;
            SelectedIndex = (int)Mathf.Repeat(m_SelectedIndex + delta, m_ThrowableSlots.Count);
        }

        public Throwable GetThrowableAtIndex(int index)
        {
            if (m_ThrowableSlots.Count == 0)
                return null;

            if (index >= m_ThrowableSlots.Count || index < 0)
            {
                Debug.LogError("Index outside of range");
                return null;
            }

            if (m_ThrowableSlots[index].Slots.Count > 0)
            {
                var wieldableItem = m_InventorySelection.GetWieldableItemWithId(m_ThrowableSlots[index].Slots[0].Item.Id);
                return (Throwable)wieldableItem.Wieldable;
            }

            return null;
        }

        public int GetThrowableCountAtIndex(int index)
        {
            if (m_ThrowableSlots.Count > index)
            {
                int count = 0;

                foreach (var slot in m_ThrowableSlots[index].Slots)
                    count += slot.Item.StackCount;

                return count;
            }

            return 0;
        }

        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_InventorySelection);
            GetModule(out m_Controller);

            for (int i = 0; i < m_ContainerTags.Length; i++)
            {
                var containersWithTag = Character.Inventory.GetContainersWithTag(m_ContainerTags[i]);

                for (int j = 0; j < containersWithTag.Count; j++)
                {
                    containersWithTag[j].SlotChanged += OnSlotChanged;
                    m_Containers.Add(containersWithTag[j]);
                }
            }

            for (int i = 0; i < m_Containers.Count; i++)
            {
                var slots = m_Containers[i].Slots;

                for (int j = 0; j < slots.Count; j++)
                {
                    if (slots[j].HasItem)
                    {
                        int itemId = slots[j].Item.Id;
                        if (m_InventorySelection.HasWieldableWithId(itemId))
                            AddSlot(slots[j]);
                    }
                }
            }
        }

        protected override void OnBehaviourDisabled()
        {
            foreach (var container in m_Containers)
                container.SlotChanged -= OnSlotChanged;
        }

        private void OnSlotChanged(ItemSlot.CallbackContext context)
        {
            if (context.Slot.HasItem && m_InventorySelection.GetWieldableItemWithId(context.Slot.Item.Id)?.Wieldable is Throwable)
            {
                if (context.Type == ItemSlot.CallbackType.StackChanged)
                    OnThrowableCountChanged();

                if (AddSlot(context.Slot))
                    OnThrowableCountChanged();
            }
            else if (RemoveSlot(context.Slot))
                OnThrowableCountChanged();
        }

        private bool AddSlot(ItemSlot slot)
        {
            int itemId = slot.Item.Id;
            for (int i = 0; i < m_ThrowableSlots.Count; i++)
            {
                if (m_ThrowableSlots[i].ItemId == itemId)
                {
                    if (m_ThrowableSlots[i].Slots.Contains(slot))
                        return false;

                    m_ThrowableSlots[i].Slots.Add(slot);
                    return true;
                }
            }

            m_ThrowableSlots.Add(new ThrowableSlot(itemId, slot));

            return true;
        }

        private bool RemoveSlot(ItemSlot slot)
        {
            for (int i = 0; i < m_ThrowableSlots.Count; i++)
            {
                if (m_ThrowableSlots[i].Slots.Remove(slot))
                    return true;
            }

            return false;
        }

        private void OnThrowableCountChanged()
        {
            ThrowableCountChanged?.Invoke();
            SelectedIndex = Mathf.Min(m_SelectedIndex, m_ThrowableSlots.Count);
        }
    }
}