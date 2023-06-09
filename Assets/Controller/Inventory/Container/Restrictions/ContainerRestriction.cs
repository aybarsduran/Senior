using IdenticalStudios.InventorySystem;

namespace IdenticalStudios.InventorySystem
{
    [System.Serializable]
    public abstract class ContainerRestriction
    {
        protected IItemContainer itemContainer;


        /// <summary>
        /// Initializes this restriction.
        /// </summary>
        /// <param name="container"></param>
        public virtual void OnInitialized(IItemContainer container) => itemContainer = container;

        /// <summary>
        /// Returns the amount of items that can be added of the given item and count.
        /// </summary>
        public abstract int GetAllowedAddAmount(IItem item, int count);

        /// <summary>
        /// Returns the amount of items that can be removed of the given item and count.
        /// </summary>
        public abstract int GetAllowedRemoveAmount(IItem item, int count);

        /// <summary>
        /// Returns a string that specifies why the item cannot be added.
        /// </summary>
        public virtual string GetRejectionString() => "Item is not valid";
    }
}
