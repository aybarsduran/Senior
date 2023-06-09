using IdenticalStudios.InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
    public sealed class ItemContainer : IItemContainer
    {
        public ItemSlot this[int i] => m_Slots[i];
        public IReadOnlyList<ItemSlot> Slots => m_Slots;
        public int Capacity => m_Slots.Count;
        public string Name => m_Name;

        public ContainerRestriction[] Restrictions => m_Restrictions;

        public event UnityAction ContainerChanged;
        public event ItemSlotChangedDelegate SlotChanged;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private List<ItemSlot> m_Slots;

        [SerializeField]
        private ContainerRestriction[] m_Restrictions;

        private static readonly DummyItem s_DummyItem = new();


        public ItemContainer(string name, int size, params ContainerRestriction[] restrictions)
        {
            m_Name = name;

            // Initialize and populate the slots.
            m_Slots = new List<ItemSlot>(size);
            for (int i = 0; i < m_Slots.Capacity; i++)
                m_Slots.Add(new ItemSlot(this));

            // Initialize the restrictions.
            m_Restrictions = restrictions;
            foreach (var restriction in m_Restrictions)
                restriction.OnInitialized(this);

            CreateSlots();
        }

        public void OnLoad()
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                var slot = m_Slots[i];

                slot = new ItemSlot(slot.Item, this);
                slot.ItemChanged += OnSlotChanged;

                m_Slots[i] = slot;
            }

            foreach (var restriction in m_Restrictions)
                restriction.OnInitialized(this);
        }

        public void SetCapacity(int capacity)
        {
            if (capacity == Capacity)
                return;

            if (capacity > Capacity)
            {
                m_Slots.Capacity = capacity;
                return;
            }

            if (capacity < Capacity)
            {
                int removeCount = Capacity - capacity + 1;
                m_Slots.RemoveRange(capacity - 1, removeCount);
            }
        }

        private void CreateSlots()
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i] = new ItemSlot(this);
                m_Slots[i].ItemChanged += OnSlotChanged;
            }
        }

        private void OnSlotChanged(ItemSlot.CallbackContext context)
        {
            if (context.Type == ItemSlot.CallbackType.PropertyChanged)
                return;

            ContainerChanged?.Invoke();
            SlotChanged?.Invoke(context);
        }

        IEnumerator IEnumerable.GetEnumerator() => m_Slots.GetEnumerator();
        public IEnumerator<ItemSlot> GetEnumerator() => m_Slots.GetEnumerator();

        public int AddItem(int id, int amount)
        {
            if (ItemDefinition.TryGetWithId(id, out var itemDef))
                return AddItemWithDefinition(itemDef, amount);

            return 0;
        }

        public int AddItem(IItem item)
        {
            if (GetAllowedCount(item, item.StackCount) < 1)
                return 0;

            // If the item is stackable try to stack it with other items.
            if (item.Definition.StackSize > 1)
            {
                int stackAddedCount = AddItemWithDefinition(item.Definition, item.StackCount);
                item.StackCount -= stackAddedCount;

                return stackAddedCount;
            }

            // The item's not stackable, try find an empty slot for it.
            foreach (var slot in m_Slots)
            {
                if (!slot.HasItem)
                {
                    slot.Item = item;
                    return 1;
                }
            }

            return 0;
        }

        private int AddItemWithDefinition(ItemDefinition itemDef, int amount)
        {
            int allowedAmount = GetAllowedCount(itemDef, amount);
            int added = 0;

            // Go through each slot and see where we can add the item(s)
            foreach (var slot in m_Slots)
            {
                added += AddItemToSlot(slot, itemDef, allowedAmount - added);

                // We've added all the items, we can stop now
                if (added == allowedAmount)
                    return added;
            }

            return added;
        }

        private int AddItemToSlot(ItemSlot slot, ItemDefinition itemDef, int amount)
        {
            if (slot.HasItem)
            {
                // If the slot already has an item in it or the item is not of the same type return.
                if (itemDef.Id != slot.Item.Id)
                    return 0;

                // Add to stack.
                return slot.Item.ChangeStack(amount);
            }

            // If the slot is empty, create a new item.
            slot.Item = new Item(itemDef, amount);
            return slot.Item.StackCount;
        }

        public bool RemoveItem(IItem item)
        {
            foreach (var slot in m_Slots)
            {
                if (slot.Item == item)
                {
                    slot.Item = null;
                    return true;
                }
            }

            return false;
        }

        public int RemoveItem(int id, int amount)
        {
            int removed = 0;

            foreach (var slot in m_Slots)
            {
                if (!slot.HasItem || slot.Item.Id != id)
                    continue;

                removed += slot.Item.ChangeStack(-(amount - removed));

                // We've removed all the items, we can stop now
                if (removed == amount)
                    return removed;
            }

            return removed;
        }

        public bool ContainsItem(IItem item)
        {
            foreach (var slot in m_Slots)
            {
                if (slot.Item == item)
                    return true;
            }

            return false;
        }

        public int GetItemCount(int id)
        {
            int count = 0;

            foreach (var slot in m_Slots)
            {
                if (slot.HasItem && slot.Item.Id == id)
                    count += slot.Item.StackCount;
            }

            return count;
        }

        public int GetAllowedCount(IItem item, int count)
        {
            if (item == null)
                return 0;

            int allowAmount = count;
            foreach (var restriction in m_Restrictions)
            {
                allowAmount = restriction.GetAllowedAddAmount(item, allowAmount);

                if (allowAmount <= 0)
                    return 0;
            }

            return allowAmount;
        }

        public int GetAllowedCount(IItem item, int count, out string rejectReason)
        {
            if (item == null)
            {
                rejectReason = "Item is NULL";
                return 0;
            }

            int allowAmount = count;
            foreach (var restriction in m_Restrictions)
            {
                allowAmount = restriction.GetAllowedAddAmount(item, allowAmount);

                if (allowAmount <= 0)
                {
                    rejectReason = restriction.GetRejectionString();
                    return 0;
                }
            }

            rejectReason = string.Empty;
            return allowAmount;
        }

        private int GetAllowedCount(ItemDefinition itemDef, int count)
        {
            if (itemDef == null)
                return 0;

            s_DummyItem.Definition = itemDef;
            return GetAllowedCount(s_DummyItem, count);
        }
    }
}