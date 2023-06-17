using IdenticalStudios.InventorySystem;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class CraftingUI : PlayerUIBehaviour
    {
        [SerializeField, NotNull]
        private SelectableGroupBaseUI m_CraftingLevelsGroup;

        [SpaceArea]

        [SerializeField, NotNull]
        private Transform m_ItemsSpawnRoot;

        [SerializeField, NotNull]
        private ItemDefinitionSlotUI m_ItemSlotTemplate;

        [SerializeField, Range(5, 20)]
        private int m_MaxTemplateInstanceCount = 10;

        private ICraftingManager m_CraftingManager;
        private int m_CurrentCraftingLevel = -1;

        private ItemDefinitionSlotUI[] m_CachedSlots;

        /// <summary>
        /// <para> Key: Crafting level. </para>
        /// Value: List of items that correspond to the crafting level.
        /// </summary>
        private readonly Dictionary<int, List<ItemDefinition>> m_CraftableItemsDictionary = new();
        private int m_CraftingItemsCount;


        public void SetAvailableCraftingLevels(params int[] levels)
        {
            var craftingLevels = m_CraftingLevelsGroup.RegisteredSelectables;

            for (int i = 0; i < craftingLevels.Count; i++)
            {
                if (IsPartOfArray(i, levels))
                    craftingLevels[i].gameObject.SetActive(true);
                else
                    craftingLevels[i].gameObject.SetActive(false);
            }

            var highestCraftableLevel = craftingLevels[GetLargestValue(levels)];
            m_CraftingLevelsGroup.SelectSelectable(highestCraftableLevel);
        }

        public void ResetCraftingLevel() => SetAvailableCraftingLevels(0);

        protected override void OnAttachment()
        {
            GetModule(out m_CraftingManager);

            var inventoryInspection = GetModule<IInventoryInspectManager>();
            inventoryInspection.AfterInspectionStarted += OnInspectionStarted;
            inventoryInspection.BeforeInspectionEnded += OnInspectionEnded;

            InitializeDictionary();
            InitializeCraftingSlots();

            m_CraftingLevelsGroup.SelectedChanged += OnCraftingLevelSelected;
            SetAvailableCraftingLevels(0);

            void OnCraftingLevelSelected(SelectableUI selectable)
            {
                int craftLevel = m_CraftingLevelsGroup.GetIndexOfSelectable(selectable);
                UpdateCraftingLevel(craftLevel);
            }
        }

        protected override void OnDetachment()
        {
            var inventoryInspection = GetModule<IInventoryInspectManager>();
            inventoryInspection.AfterInspectionStarted -= RefreshSlots;
            inventoryInspection.BeforeInspectionEnded -= RefreshSlots;
        }

        private void OnInspectionStarted()
        {
            Player.Inventory.InventoryChanged += RefreshSlots;
            RefreshSlots();
        }

        private void OnInspectionEnded()
        {
            Player.Inventory.InventoryChanged -= RefreshSlots;
            RefreshSlots();
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < m_CachedSlots.Length; i++)
            {
                var slot = m_CachedSlots[i];
                if (slot.gameObject.activeSelf)
                    slot.SetItem(slot.ItemDef);
            }
        }

        private void UpdateCraftingLevel(int level)
        {
            if (level == m_CurrentCraftingLevel)
                return;

            if (m_CraftableItemsDictionary.TryGetValue(level, out List<ItemDefinition> items))
            {
                SetCurrentlyCraftableItems(items);
                m_CurrentCraftingLevel = level;
            }
        }

        private void SetCurrentlyCraftableItems(List<ItemDefinition> items)
        {
            if (items == null)
            {
                for (int i = 0; i < m_CachedSlots.Length; i++)
                    m_CachedSlots[i].SetNull();
                return;
            }

            int enableCount = items.Count < m_CachedSlots.Length ? items.Count : m_CachedSlots.Length;

            for (int i = 0; i < enableCount; i++)
                m_CachedSlots[i].SetItem(items[i]);

            // Hide the remainning slots.
            if (enableCount < m_CachedSlots.Length)
            {
                int slotStartIndex = items.Count;

                for (int i = slotStartIndex; i < m_CachedSlots.Length; i++)
                    m_CachedSlots[i].SetNull();
            }
        }

        private void InitializeDictionary()
        {
            int craftableItemsCount = 0;

            foreach (var item in ItemDefinition.Definitions)
            {   
                if (item.TryGetDataOfType<CraftingData>(out var data) && data.IsCraftable)
                {
                    if (m_CraftableItemsDictionary.TryGetValue(data.CraftLevel, out var list))
                    {
                        list.Add(item);
                        Debug.Log(item.name);
                        Debug.Log(data.CraftLevel);
                    }
                    else
                    {
                        list = new List<ItemDefinition>() { item };
                        m_CraftableItemsDictionary.Add(data.CraftLevel, list);
                    }

                    craftableItemsCount++;
                }
            }

            m_CraftingItemsCount = craftableItemsCount;
        }

        private void InitializeCraftingSlots()
        {    
            int instancesCount = Mathf.Min(m_MaxTemplateInstanceCount, m_CraftingItemsCount);
            m_CachedSlots = new ItemDefinitionSlotUI[instancesCount];
            var spawnRoot = m_ItemsSpawnRoot.transform;
            for (int i = 0; i < instancesCount; i++)
            {
                var slot = Instantiate(m_ItemSlotTemplate, spawnRoot);
                slot.SetNull();
                slot.Selectable.OnSelected += StartCrafting;

                m_CachedSlots[i] = slot;
            }

            void StartCrafting(SelectableUI selectable)
            {
                var itemSlot = selectable.gameObject.GetComponent<ItemDefinitionSlotUI>();
                m_CraftingManager.Craft(itemSlot.ItemDef);
            }
        }

        private static bool IsPartOfArray(int refInt, int[] range)
        {
            for (int i = 0; i < range.Length; i++)
            {
                if (refInt == range[i])
                    return true;
            }

            return false;
        }

        private static int GetLargestValue(int[] intArray)
        {
            int highestValue = -1000000000;

            for (int i = 0; i < intArray.Length; i++)
            {
                if (intArray[i] > highestValue)
                    highestValue = intArray[i];
            }

            return highestValue;
        }
    }
}