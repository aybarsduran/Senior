using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof(SelectableUI))]
    public class ItemCategorySlotUI : SlotUI<ItemCategoryDefinition>
    {
        public ItemCategoryDefinition ItemCategory => Data;


        public void SetCategory(ItemCategoryDefinition category)
        {
            SetData(category);
        }
    }
}
