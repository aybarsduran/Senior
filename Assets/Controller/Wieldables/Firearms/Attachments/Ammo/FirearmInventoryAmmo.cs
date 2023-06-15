using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Ammo/Inventory Ammo")]
    public class FirearmInventoryAmmo : FirearmAmmoBehaviour
    {
        [SpaceArea]
        [SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<ItemDefinition> m_AmmoItem;

        private IInventory m_Inventory;
 

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Inventory = Wieldable.Character.Inventory;
            m_Inventory.ItemChanged += OnInventoryChanged;
        }

        protected override void OnDisable()
        {
            if (m_Inventory != null)
                m_Inventory.ItemChanged -= OnInventoryChanged;
        }

        private void OnInventoryChanged(ItemSlot.CallbackContext context)
        {
            if (context.Type != ItemSlot.CallbackType.PropertyChanged)
                RaiseAmmoChangedEvent(GetAmmoCount());
        }

        public override int RemoveAmmo(int amount) => m_Inventory.RemoveItemsWithName(m_AmmoItem, amount);
        public override int AddAmmo(int amount) => m_Inventory.AddItemsWithName(m_AmmoItem, amount);
        public override int GetAmmoCount() => m_Inventory.GetItemsWithIdCount(m_AmmoItem);
    }
}