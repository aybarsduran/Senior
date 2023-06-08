using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class WieldableItemBehaviour : MonoBehaviour
    {
        public IWieldableItem Item { get; private set; }


        protected virtual void Awake()
        {
            Item = GetComponentInParent<IWieldableItem>();

            if (Item == null)
            {
                Debug.LogError($"No wieldable item found for {gameObject.name}.");
                return;
            }

            Item.AttachedItemChanged += OnItemChanged;
        }

        protected virtual void OnItemChanged(IItem item) { }
    }
}
