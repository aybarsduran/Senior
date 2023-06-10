using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    [DisallowMultipleComponent]
    public sealed class Inventory : MonoBehaviour, IInventory, ISaveableComponent
    {
        public IReadOnlyList<IItemContainer> Containers => m_Containers;
        public ContainerGenerator[] StartupContainers => m_StartupContainers;

        public event UnityAction InventoryChanged;
        public event ItemSlotChangedDelegate ItemChanged;
        public event UnityAction ContainersCountChanged;

        [SerializeField, ReorderableList]
        [Tooltip("The initial item containers")]
        private ContainerGenerator[] m_StartupContainers;
        
        private List<IItemContainer> m_Containers;
        
        
        public void AddContainer(IItemContainer container, bool triggerContainersEvent = true)
        {
            if (m_Containers.Contains(container))
                return;
            
            m_Containers.Add(container);

            container.ContainerChanged += OnInventoryChanged;
            container.SlotChanged += OnItemChanged;

            if (triggerContainersEvent)
                ContainersCountChanged?.Invoke();
        }

        public void RemoveContainer(IItemContainer container, bool triggerContainersEvent = true)
        {
            if (!m_Containers.Remove(container))
                return;

            container.ContainerChanged += OnInventoryChanged;
            container.SlotChanged += OnItemChanged;

            if (triggerContainersEvent)
                ContainersCountChanged?.Invoke();
        }

        public int AddItem(IItem item)
        {
            int addedInTotal = 0;

            foreach (var container in m_Containers)
            {
                addedInTotal += container.AddItem(item);

                if (addedInTotal >= item.StackCount)
                    break;
            }

            return addedInTotal;
        }

        public int AddItemsWithId(int itemId, int amountToAdd)
        {
            if (amountToAdd <= 0)
                return 0;
            
            int addedInTotal = 0;
            
            foreach (var container in m_Containers)
            {
                int added = container.AddItem(itemId, amountToAdd);
                addedInTotal += added;

                if (added == addedInTotal)
                    break;
            }

            return addedInTotal;
        }

        public bool RemoveItem(IItem item)
        {
            foreach (var container in m_Containers)
            {
                if (container.RemoveItem(item))
                    return true;
            }

            return false;
        }

        public int RemoveItemsWithId(int itemId, int amountToRemove)
        {
            if (amountToRemove <= 0)
                return 0;

            int removedInTotal = 0;

            foreach (var container in m_Containers)
            {
                int removedNow = container.RemoveItem(itemId, amountToRemove);
                removedInTotal += removedNow;

                if (removedNow == removedInTotal)
                    break;
            }

            return removedInTotal;
        }

        public int GetItemsWithIdCount(int itemId)
        {
            int count = 0;

            foreach (var container in m_Containers)
                count += container.GetItemCount(itemId);

            return count;
        }

        public bool ContainsItem(IItem item)
        {
            foreach (var container in m_Containers)
            {
                if (container.ContainsItem(item))
                    return true;
            }

            return false;
        }

        private void RemoveAllContainers()
        {
            if (m_Containers == null)
                return;

            foreach (var container in m_Containers)
            {
                container.ContainerChanged -= OnInventoryChanged;
                container.SlotChanged -= OnItemChanged;
            }

            m_Containers.Clear();
        }

        private void Awake()
        {
            if (m_Containers != null)
            {
#if !UNITY_EDITOR
            m_StartupContainers = null;
#endif
                return;
            }

            m_Containers = new List<IItemContainer>();
            for (int i = 0; i < m_StartupContainers.Length; i++)
            {
                var container = m_StartupContainers[i].GenerateContainer();
                AddContainer(container, false);
            }

#if !UNITY_EDITOR
            m_StartupContainers = null;
#endif
        }

        private void OnInventoryChanged() => InventoryChanged?.Invoke();
        private void OnItemChanged(ItemSlot.CallbackContext context) => ItemChanged?.Invoke(context);

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            var savedContainers = (List<IItemContainer>)members[0];

            RemoveAllContainers();

            foreach (var container in savedContainers)
            {
                ((ItemContainer)container).OnLoad();
                AddContainer(container, false);
            }
        }

        public object[] SaveMembers()
        {
            return new object[]
            {
                m_Containers
            };
        }
        #endregion
    }
}