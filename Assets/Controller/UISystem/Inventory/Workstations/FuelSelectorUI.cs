using IdenticalStudios.InventorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class FuelSelectorUI : MonoBehaviour
    {
        #region Internal
        public class FuelItem
        {
            public DataIdReference<ItemDefinition> Item;

            public int Count;
            public readonly float Duration;


            public FuelItem(int id, int count, float duration)
            {
                Item = id;
                Count = count;
                Duration = duration;
            }
        }
        #endregion

        public FuelItem SelectedFuel { get; private set; }

        [SerializeField]
        private Button m_NextBtn;

        [SerializeField]
        private Button m_PreviousBtn;

        [SerializeField]
        private Image m_IconImg;

        private FuelItem[] m_FuelItems = Array.Empty<FuelItem>();
        private int m_SelectedFuelIdx;

        private IInventory m_Inventory;


        public void AttachToInventory(IInventory inventory) 
        {
            m_Inventory = inventory;
            inventory.InventoryChanged += OnInventoryChanged;
            OnInventoryChanged();
        }

        public void DetachFromInventory() 
        {
            m_Inventory.InventoryChanged -= OnInventoryChanged;
            m_Inventory = null;
        }

        private void Awake()
        {
            CacheFuelItems();
            m_SelectedFuelIdx = 0;

            m_NextBtn.onClick.AddListener(()=> SelectNextFuel(true));
            m_PreviousBtn.onClick.AddListener(()=> SelectNextFuel(false));

            SelectFuelAtIndex(0);
        }

        private void OnInventoryChanged()
        {
            RefreshFuelList();

            if (m_SelectedFuelIdx == -1 || m_FuelItems[m_SelectedFuelIdx].Count == 0)
                SelectNextFuel(true);
        }

        private void SelectNextFuel(bool selectNext)
        {
            RefreshFuelList();

            bool foundValidFuel = false;
            int iterations = 0;
            int i = m_SelectedFuelIdx;

            do
            {
                i = (int)Mathf.Repeat(i + (selectNext ? 1 : -1), m_FuelItems.Length);
                iterations++;

                if (m_FuelItems[i].Count > 0)
                {
                    foundValidFuel = true;
                    m_SelectedFuelIdx = i;
                }
            }
            while(!foundValidFuel && iterations < m_FuelItems.Length);

            m_SelectedFuelIdx = foundValidFuel ? i : -1;
            SelectFuelAtIndex(m_SelectedFuelIdx);
        }

        private void SelectFuelAtIndex(int index)
        {
            if (m_FuelItems == null || m_FuelItems.Length < 1)
                return;

            m_IconImg.enabled = index > -1;

            if (index > -1)
            {
                SelectedFuel = m_FuelItems[index];

                if (ItemDefinition.TryGetWithId(SelectedFuel.Item, out var itemDef))
                    m_IconImg.sprite = SelectedFuel.Item.Icon;
            }
        }

        private void CacheFuelItems()
        {
            var fuelItems = new List<FuelItem>();

            foreach (var itemDef in ItemDefinition.Definitions)
            {
                if (itemDef.TryGetDataOfType(out FuelData fuel))
                    fuelItems.Add(new FuelItem(itemDef.Id, 0, fuel.FuelEfficiency));
            }

            m_FuelItems = fuelItems.ToArray();
        }

        private void RefreshFuelList()
        {
            foreach (var item in m_FuelItems)
                item.Count = 0;

            var containers = m_Inventory.Containers;
            foreach (var container in containers)
            {
                if (container.HasContainerRestriction<ContainerTagRestriction>())
                    return;

                for (int i = 0; i < container.Capacity; i++)
                {
                    IItem item = container.Slots[i].Item;
                    if (item != null && item.Definition.HasDataOfType(typeof(FuelData)) && TryGetFuelItem(item.Id, out FuelItem fuelItem))
                        fuelItem.Count += item.StackCount;
                }
            }
        }

        private bool TryGetFuelItem(int itemId, out FuelItem fuelItem)
        {
            foreach (var item in m_FuelItems)
            {
                if (item.Item == itemId)
                {
                    fuelItem = item;
                    return true;
                }
            }

            fuelItem = null;
            return false;
        }
    }
}