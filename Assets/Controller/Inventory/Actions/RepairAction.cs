using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    /// TODO: Implement
    public sealed class RepairAction : ItemAction
    {
        [Title("Repairing")]

        [SerializeField, Range(0f, 25f)]
        [Tooltip("The time it takes to repair an item at this station.")]
        private float m_RepairDuration = 1f;

        [SerializeField]
        [Tooltip("The id of the durability property. After repairing an item the workbench will increase the value of that property for the repaired item.")]
        private DataIdReference<ItemPropertyDefinition> m_DurabilityProperty;

        [SerializeField]
        [Tooltip("Repair sound to be played after successfully repairing an item.")]
        private SoundPlayer m_RepairAudio;


        public override bool IsViableForItem(ICharacter character, ItemSlot itemSlot)
        {
            IItem itemToRepair = itemSlot.Item;

            if (itemToRepair == null)
                return false;

            bool isViable = itemToRepair.HasPropertyWithId(m_DurabilityProperty) && itemToRepair.GetPropertyWithId(m_DurabilityProperty).Float < 100f;

            return isViable;
        }

        public override float GetDuration(ICharacter character, ItemSlot itemSlot) => m_RepairDuration;

        public override void PerformAction(ICharacter character, ItemSlot itemSlot)
        {
            if (itemSlot.Item != null)
            {
                bool canRepairItem = itemSlot.Item.TryGetPropertyWithId(m_DurabilityProperty, out IItemProperty durabilityProperty);
                canRepairItem &= itemSlot.Item.Definition.TryGetDataOfType<CraftingData>(out var CraftData);

                if (canRepairItem)
                {
                    float durability = durabilityProperty.Float;
                    var craftReq = CraftData.CreateCraftRequirements(durability);
                }
            }
        }

        public override void CancelAction(ICharacter character, ItemSlot itemSlot)
        {

        }
    }
}