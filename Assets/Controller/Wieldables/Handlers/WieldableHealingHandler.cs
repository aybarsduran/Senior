using IdenticalStudios.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public class WieldableHealingHandler : CharacterBehaviour, IWieldableHealingHandler
    {
        public event UnityAction HealsCountChanged;
        public event UnityAction<HealingItem> OnHeal;

        [SerializeField, DataReferenceDetails(NullElementName = "Untagged", HasLabel = false), ReorderableList]
        private DataIdReference<ItemTagDefinition>[] m_ContainerTags;

        [SpaceArea]

        [SerializeField]
        private bool m_BlockRunning;

        [SerializeField]
        private bool m_BlockJumping;

        private readonly List<IItemContainer> m_Containers = new();
        private readonly List<ItemSlot> m_HealSlots = new();

        private IWieldableSelectionHandler m_InventorySelection;
        private IWieldablesController m_Controller;
        private IMovementController m_Movement;


        public bool TryHeal()
        {
            if (!IsBehaviourEnabled || m_Controller.IsEquipping || m_HealSlots.Count == 0)
                return false;

            var selectedSlot = m_HealSlots[0];
            if (selectedSlot != null && selectedSlot.HasItem)
            {
                var healingItem = (HealingItem)m_InventorySelection.GetWieldableItemWithId(selectedSlot.Item.Id).Wieldable;
                if (m_Controller.TryEquipWieldable(healingItem))
                {
                    if (m_BlockRunning) m_Movement.AddStateLocker(this, MovementStateType.Run);
                    if (m_BlockJumping) m_Movement.AddStateLocker(this, MovementStateType.Jump);

                    healingItem.Use(UsePhase.Start);

                    m_Controller.WieldableHolsterStarted += OnHealingItemHolster;
                    OnHeal?.Invoke(healingItem);

                    return true;
                }
            }

            return false;

            void OnHealingItemHolster(IWieldable equippedWieldable)
            {
                if (m_BlockRunning) m_Movement.RemoveStateLocker(this, MovementStateType.Run);
                if (m_BlockJumping) m_Movement.RemoveStateLocker(this, MovementStateType.Jump);

                m_Controller.WieldableHolsterStarted -= OnHealingItemHolster;
            }
        }

        public int GetHealsCount()
        {
            int count = 0;

            for (int i = 0; i < m_HealSlots.Count; i++)
                count += m_HealSlots[i].Item?.StackCount ?? 0;

            return count;
        }

        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_InventorySelection);
            GetModule(out m_Controller);
            GetModule(out m_Movement);

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
                    if (slots[j].HasItem && m_InventorySelection.HasWieldableWithId(slots[j].Item.Id))
                        m_HealSlots.Add(slots[j]);
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
            if (context.Slot.HasItem && m_InventorySelection.GetWieldableItemWithId(context.Slot.Item.Id)?.Wieldable is HealingItem)
            {
                if (context.Type == ItemSlot.CallbackType.StackChanged)
                    HealsCountChanged?.Invoke();

                if (!m_HealSlots.Contains(context.Slot))
                {
                    m_HealSlots.Add(context.Slot);
                    HealsCountChanged?.Invoke();
                }
            }
            else if (m_HealSlots.Remove(context.Slot))
                HealsCountChanged?.Invoke();
        }
    }
}