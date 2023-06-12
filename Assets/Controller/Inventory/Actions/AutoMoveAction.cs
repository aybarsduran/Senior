using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Actions/Auto Move Action", fileName = "(Action) Auto Move")]
    public sealed class AutoMoveAction : ItemAction
    {
        [Title("Auto Moving")]

        [SerializeField, ReorderableList(childLabel: "Tag")]
        [Tooltip("All of the item tag(s) that correspond to the wieldables.")]
        private DataIdReference<ItemTagDefinition> m_WieldableTag;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        [Tooltip("All of the clothing tag(s) that correspond to clothing items.")]
        private DataIdReference<ItemTagDefinition>[] m_ClothingTags;

        private IInventoryInspectManager m_CachedInspector;
        private IInventory m_CachedInventory;


        public override float GetDuration(ICharacter character, ItemSlot itemSlot) => 0f;
        public override bool IsViableForItem(ICharacter character, ItemSlot itemSlot) => true;

        public override void PerformAction(ICharacter character, ItemSlot itemSlot)
        {
            if (!itemSlot.HasItem || !itemSlot.HasContainer)
                return;

            m_CachedInventory = character.Inventory;
            m_CachedInspector = character.GetModule<IInventoryInspectManager>();

            // Get the necessary references.
            IItemContainer container = itemSlot.Container;
            var tagRestriction = container.GetContainerRestriction<ContainerTagRestriction>();
            var validTags = tagRestriction?.ValidTags;

            // Move item from EXTERNAL container to somewhere else.
            if (IsContainerExternal(container))
            {
                if (TryMoveToStorage(itemSlot, container)) return;
                if (TryMoveToHolster(itemSlot, container)) return;
                if (TryMoveToEquipment(itemSlot, container)) return;
            }
            // Move item from HOLSTER to somewhere else.
            else if (validTags != null && validTags.ContainsDef(m_WieldableTag))
            {
                if (TryMoveToExternal(itemSlot, container)) return;
                if (TryMoveToStorage(itemSlot, container)) return;
            }
            // Move item from EQUIPMENT to somewhere else.
            else if (validTags != null && validTags.ContainsAnyDef(m_ClothingTags))
            {
                if (TryMoveToExternal(itemSlot, container)) return;
                if (TryMoveToStorage(itemSlot, container)) return;
            }
            // Move item from STORAGE to somewhere else.
            else
            {
                if (TryMoveToExternal(itemSlot, container)) return;
                if (TryMoveToEquipment(itemSlot, container)) return;
                if (TryMoveToHolster(itemSlot, container)) return;
            }

            m_CachedInventory = null;
            m_CachedInspector = null;
        }

        // Try to move the item from its parent container to a generic storage container.
        private bool TryMoveToStorage(ItemSlot itemSlot, IItemContainer parentContainer)
        {
            foreach (var container in m_CachedInventory.GetContainersWithoutTag())
            {
                if (parentContainer.AddOrSwapItem(itemSlot, container))
                    return true;
            }

            return false;
        }

        // Try to move the item from its parent container to a container that holds wieldables.
        private bool TryMoveToHolster(ItemSlot itemSlot, IItemContainer parentContainer)
        {
            if (IsWieldable(itemSlot))
            {
                foreach (var container in m_CachedInventory.GetContainersWithTag(m_WieldableTag))
                {
                    if (parentContainer.AddOrSwapItem(itemSlot, container))
                        return true;
                }
            }

            return false;
        }

        // Try to move the item from its parent container to a container that holds clothes/equipment.
        private bool TryMoveToEquipment(ItemSlot itemSlot, IItemContainer parentContainer)
        {
            if (IsClothing(itemSlot))
            {
                foreach (var clothingTag in m_ClothingTags)
                {
                    foreach (var container in m_CachedInventory.GetContainersWithTag(clothingTag))
                    {
                        if (parentContainer.AddOrSwapItem(itemSlot, container))
                            return true;
                    }
                }
            }

            return false;
        }

        // Try to move the item from its parent container to an external container (if active).
        private bool TryMoveToExternal(ItemSlot itemSlot, IItemContainer parentContainer)
        {
            if (GetExternalContainer() != null)
                return parentContainer.AddOrSwapItem(itemSlot, GetExternalContainer());

            return false;
        }

        private bool IsContainerExternal(IItemContainer container)
        {
            var externalContainer = GetExternalContainer();

            if (externalContainer == null)
                return false;

            return container == externalContainer;
        }

        private IItemContainer GetExternalContainer()
        {
            var workstation = m_CachedInspector.Workstation;

            if (workstation != null)
            {
                var containers = m_CachedInspector.Workstation.GetContainers();
                return containers.Length > 0 ? containers[0] : null;
            }

            return null;
        }

        private bool IsClothing(ItemSlot slot)
        {
            ItemDefinition info = slot.Item.Definition;

            for (int i = 0; i < m_ClothingTags.Length; i++)
            {
                if (info.Tag == m_ClothingTags[i])
                    return true;
            }

            return false;
        }

        private bool IsWieldable(ItemSlot slot)
        {
            ItemDefinition info = slot.Item.Definition;

            if (info.Tag == m_WieldableTag)
                return true;

            return false;
        }
    }
}