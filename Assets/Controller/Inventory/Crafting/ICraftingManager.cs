using IdenticalStudios.InventorySystem;
using UnityEngine.Events;

namespace IdenticalStudios
{
    // TODO: Implement unlocked recipes and crafting queue.
    public interface ICraftingManager : ICharacterModule
    {
        bool IsCrafting { get; }

        event UnityAction<ItemDefinition> CraftingStart;
        event UnityAction CraftingEnd;

        void Craft(ItemDefinition itemInfo);
        void CancelCrafting();
    }
}