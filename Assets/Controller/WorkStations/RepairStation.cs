using IdenticalStudios.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.BuildingSystem
{
    /// <summary>
    /// Convert repairing to item actions.
    /// </summary>
    public sealed class RepairStation : Workstation, ISaveableComponent
    {
        public List<CraftRequirement> RepairRequirementsForCurrentItem => m_RepairRequirements;

        public IItem ItemToRepair => m_ItemContainer.Slots[0].Item;
        public float RepairDuration => m_RepairDuration;

        public event UnityAction ItemToRepairChanged;

        [Title("Settings (Repair Station)")]

        [SerializeField, Range(0f, 25f)]
        [Tooltip("The time it takes to repair an item at this station.")]
        private float m_RepairDuration = 1f;

        [SerializeField]
        [Tooltip("The id of the durability property. After repairing an item the workbench will increase the value of that property for the repaired item.")]
        private DataIdReference<ItemPropertyDefinition> m_DurabilityProperty;

        [SpaceArea]

        [SerializeField]
        [Tooltip("Repair sound to be played after successfully repairing an item.")]
        private SoundPlayer m_RepairAudio;

        private ItemContainer m_ItemContainer;
        private List<CraftRequirement> m_RepairRequirements;


        public override IItemContainer[] GetContainers() => new IItemContainer[] { m_ItemContainer };

        public bool CanRepairItem()
        {
            if (ItemToRepair == null)
                return false;

            IItem itemToRepair = m_ItemContainer.Slots[0].Item;
            bool canRepairItem = itemToRepair != null && itemToRepair.HasPropertyWithId(m_DurabilityProperty) && itemToRepair.GetPropertyWithId(m_DurabilityProperty).Float < 100f;

            return canRepairItem;
        }

        public void RepairItem(ICharacter character)
        {
            foreach (var req in m_RepairRequirements)
                character.Inventory.RemoveItemsWithName(req.Item, req.Amount);

            ItemToRepair.GetPropertyWithId(m_DurabilityProperty).Float = 100f;

            m_RepairAudio.PlayAtPosition(transform.position, 1f);
        }

        private void Start()
        {
            if (m_ItemContainer == null)
                GenerateContainer();
        }

        private void GenerateContainer()
        {
            m_ItemContainer = new ItemContainer("Workspace", 1, new ContainerPropertyRestriction(m_DurabilityProperty));
            m_RepairRequirements = new List<CraftRequirement>();
            m_ItemContainer.SlotChanged += OnContainerChanged;
        }

        private void OnContainerChanged(ItemSlot.CallbackContext context)
        {
            if (context.Slot.HasItem)
            {
                var slot = context.Slot;
                bool canRepairItem = slot.Item.TryGetPropertyWithId(m_DurabilityProperty, out IItemProperty durabilityProperty);
                canRepairItem &= slot.Item.Definition.TryGetDataOfType<CraftingData>(out var CraftData);

                if (canRepairItem)
                {
                    m_RepairRequirements.Clear();

                    float durability = durabilityProperty.Float;

                    foreach (var req in CraftData.Blueprint)
                    {
                        int requiredAmount = Mathf.Max(Mathf.RoundToInt(req.Amount * Mathf.Clamp01((100f - durability) / 100f)), 1);
                        m_RepairRequirements.Add(new CraftRequirement(req.Item, requiredAmount));
                    }
                }
            }

            ItemToRepairChanged?.Invoke();
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_ItemContainer = (ItemContainer)members[0];
            m_RepairRequirements = (List<CraftRequirement>)members[1];

            m_ItemContainer.SlotChanged += OnContainerChanged;
            m_ItemContainer.OnLoad();
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_ItemContainer,
                m_RepairRequirements,
            };

            return members;
        }
        #endregion
    }
}