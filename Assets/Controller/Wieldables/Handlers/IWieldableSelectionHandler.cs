using IdenticalStudios.InventorySystem;
using IdenticalStudios.WieldableSystem;
using UnityEngine.Events;

namespace IdenticalStudios
{
    /// <summary>
    /// Takes care of selecting wieldables based on inventory items.
    /// </summary>
    public interface IWieldableSelectionHandler : ICharacterModule
    {
        int SelectedIndex { get; }
        int PreviousIndex { get; }

        ItemSlot ItemSlot { get; }

        event UnityAction<int> SelectedChanged;


        void SelectAtIndex(int index, float holsterPreviousSpeed = 1f);
        void DropWieldable(float dropDelay = 0.35f, bool forceDrop = false);

        bool HasWieldableWithId(int id);
        IWieldableItem GetWieldableItemWithId(int id);
    }
}