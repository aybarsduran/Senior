using IdenticalStudios.InventorySystem;

namespace IdenticalStudios.UISystem
{
    public class ItemDefinitionSlotUI : SlotUI<IItem>
    {
        public ItemDefinition ItemDef => m_ItemDefinition;

        private ItemDefinition m_ItemDefinition;
        private static readonly DummyItem s_DummyItem = new();


        public void SetNull() => SetData(null);
        public void SetItem(IItem item) => SetData(item);

        public void SetItem(ItemSlot itemSlot)
        {
            if (itemSlot != null)
            {
                SetData(itemSlot.Item);
                m_ItemDefinition = itemSlot.Item.Definition;
            }
            else
            {
                SetData(null);
                m_ItemDefinition = null;
            }
        }

        public void SetItem(ItemSlotUI itemSlotUI)
        {
            if (itemSlotUI != null)
            {
                SetData(itemSlotUI.Item);
                m_ItemDefinition = itemSlotUI.Item?.Definition;
            }
            else
            {
                SetData(null);
                m_ItemDefinition = null;
            }
        }

        public void SetItem(ItemDefinition itemDef)
        {
            s_DummyItem.Definition = itemDef;
            s_DummyItem.StackCount = 1;
            SetData(s_DummyItem);

            m_ItemDefinition = itemDef;
        }
    }
}
