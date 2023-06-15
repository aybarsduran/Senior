using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Actions/Equip Action", fileName = "(Action) Equip")]
    public sealed class EquipAction : ItemAction
    {
        [Title("Equipping")]

        [SerializeField]
        private DataIdReference<ItemTagDefinition> m_WieldableTag;


        /// <summary>
        /// Checks if this item can be equipped.
        /// </summary>
        public override bool IsViableForItem(ICharacter character, ItemSlot itemSlot)
        {
            bool isItemValid = itemSlot.HasItem && itemSlot.Item.Definition.Tag == m_WieldableTag && itemSlot.HasContainer;
            bool isContainerValid = itemSlot.HasContainer && !itemSlot.Container.HasTag(m_WieldableTag);

            return isItemValid && isContainerValid;
        }

        public override float GetDuration(ICharacter character, ItemSlot itemSlot) => 0f;

        public override void PerformAction(ICharacter character, ItemSlot itemSlot)
        {
            var parentContainer = itemSlot.Container;

            foreach (var holsterContainer in character.Inventory.GetContainersWithTag(m_WieldableTag))
            {
                if (parentContainer.AddOrSwapItem(itemSlot, holsterContainer))
                    break;
            }
        }
    }
}