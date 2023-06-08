using IdenticalStudios.WieldableSystem;
using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface IWieldableItem
    {
        IItem AttachedItem { get; }
        IWieldable Wieldable { get; }

        event UnityAction<IItem> AttachedItemChanged;

        void SetItem(IItem item);

        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        #endregion
    }
}
