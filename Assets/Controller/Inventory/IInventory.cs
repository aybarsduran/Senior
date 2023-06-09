using System.Collections.Generic;
using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    public interface IInventory : ICharacterModule
    {
        IReadOnlyList<IItemContainer> Containers { get; }

        event UnityAction InventoryChanged;
        event ItemSlotChangedDelegate ItemChanged;
        event UnityAction ContainersCountChanged;


        /// <summary>
        /// Adds a container to this inventory.
        /// </summary>
        void AddContainer(IItemContainer itemContainer, bool triggerContainersEvent = true);

        /// <summary>
        /// Removes a container from this inventory.
        /// </summary>
        void RemoveContainer(IItemContainer itemContainer, bool triggerContainersEvent = true);

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
		/// <returns> stack Added Count. </returns>
        int AddItem(IItem item);

        /// <summary>
        /// Adds an amount of items with the given id to the inventory.
        /// </summary>
        int AddItemsWithId(int itemId, int amountToAdd);

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        bool RemoveItem(IItem item);

        /// <summary>
        /// Removes an amount of items with the given id from the inventory.
        /// </summary>
        int RemoveItemsWithId(int itemId, int amountToRemove);

        /// <summary>
        /// Counts all the items with the given id, in all containers.
        /// </summary>
        int GetItemsWithIdCount(int itemId);

        /// <summary>
        /// Returns true if the item is found in any of the child containers.
        /// </summary>
        bool ContainsItem(IItem item);
    }
}