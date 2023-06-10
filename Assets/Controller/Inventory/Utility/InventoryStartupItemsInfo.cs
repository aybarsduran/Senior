using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "IdenticalStudios/Items/Startup Items", order = 10)]
    public class InventoryStartupItemsInfo : ScriptableObject
    {
        #region Internal
        [Serializable]
        public class ItemContainerStartupItems
        {
            public string Name;
            public ItemGenerator[] StartupItems;
        }
        #endregion

        [SerializeField]
        private ItemContainerStartupItems[] m_ItemContainersStartupItems;


        public void AddItemsToInventory(IInventory inventory)
        {
            foreach (var container in m_ItemContainersStartupItems)
            {
                IItemContainer itemContainer = inventory.GetContainerWithName(container.Name);

                if (itemContainer == null)
                    continue;

                var items = container.StartupItems;
                foreach (var item in items)
                    itemContainer.AddItem(item.GenerateItem());
            }
        }
    }
}