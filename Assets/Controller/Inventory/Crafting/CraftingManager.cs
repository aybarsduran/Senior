using IdenticalStudios.InventorySystem;
using IdenticalStudios.UISystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    public sealed class CraftingManager : CharacterBehaviour, ICraftingManager
    {
        public bool IsCrafting => m_CurrentItemToCraft != null;

        public event UnityAction<ItemDefinition> CraftingStart;
        public event UnityAction CraftingEnd;

        [SerializeField]
        [Tooltip("Craft Sound: Sound that will be played after crafting an item.")]
        private StandardSound m_CraftSound;

        private ItemDefinition m_CurrentItemToCraft;

        private IInventory m_Inventory;
        private IItemDropHandler m_ItemDropHandler;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_ItemDropHandler);
            GetModule(out m_Inventory);
        }

        public void Craft(ItemDefinition itemDef)
        {
            if (IsCrafting)
                return;

            if (itemDef.TryGetDataOfType<CraftingData>(out var crafData))
            {
                var blueprint = crafData.Blueprint;

                // Verify if all of the blueprint crafting materials exist in the inventory 
                for (int i = 0; i < blueprint.Length; i++)
                {
                    int itemCount = m_Inventory.GetItemsWithIdCount(blueprint[i].Item);
                    if (itemCount < blueprint[i].Amount)
                        return;
                }

                // Start crafting
                m_CurrentItemToCraft = itemDef;
                var craftingParams = new CustomActionManagerUI.AParams("Crafting", "Crafting <b>" + itemDef.Name + "</b>...", crafData.CraftDuration, true, OnCraftItemEnd, OnCraftCancel);
                CustomActionManagerUI.TryStartAction(craftingParams);

                CraftingStart?.Invoke(itemDef);

                Character.AudioPlayer.PlaySound(m_CraftSound);
            }
        }

        public void CancelCrafting()
        {
            if (IsCrafting)
                CustomActionManagerUI.TryCancelAction();
        }

        private void OnCraftItemEnd()
        {
            if (!IsCrafting)
                return;

            if (m_CurrentItemToCraft.TryGetDataOfType<CraftingData>(out var craftData))
            {
                var blueprint = craftData.Blueprint;

                // Remove the blueprint items from the inventory
                for (int i = 0; i < blueprint.Length; i++)
                {
                    int removedCount = m_Inventory.RemoveItemsWithName(blueprint[i].Item, blueprint[i].Amount);

                    if (removedCount < blueprint[i].Amount)
                        return;
                }

                int addedItems = m_Inventory.AddItemsWithId(m_CurrentItemToCraft.Id, craftData.CraftAmount);

                // if the crafted item couldn't be added to the inventory, spawn the world prefab.
                if (addedItems < craftData.CraftAmount)
                    m_ItemDropHandler.DropItem(new Item(m_CurrentItemToCraft, craftData.CraftAmount - addedItems));

                m_CurrentItemToCraft = null;
                CraftingEnd?.Invoke();
            }
        }

        private void OnCraftCancel()
        {
            m_CurrentItemToCraft = null;
            CraftingEnd?.Invoke();
        }
    }
}