using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Actions/Dismantle Action", fileName = "(Action) Dismantle")]
    public sealed class DismantleAction : ItemAction
    {
        [Title("Dismantling")]

        [SerializeField, Suffix("sec"), Clamp(0f, 100f)]
        private float m_DurationPerGivenItem = 2f;

        [SerializeField]
        private DataIdReference<ItemPropertyDefinition> m_DurabilityProperty;

        [SerializeField]
        private SoundPlayer m_DismantleSound;

        private CraftingData m_CraftData;


        public override bool IsViableForItem(ICharacter character, ItemSlot itemSlot)
        {
            if (itemSlot.Item.Definition.TryGetDataOfType(out m_CraftData))
                return m_CraftData.AllowDismantle;

            return false;
        }

        public override float GetDuration(ICharacter character, ItemSlot itemSlot)
        {
            if (itemSlot.Item.Definition.TryGetDataOfType(out m_CraftData))
                return m_CraftData.Blueprint.Length * m_DurationPerGivenItem;

            return 0f;
        }

        public override void StartAction(ICharacter character, ItemSlot itemSlot) => m_DismantleSound.Play2D();

        public override void PerformAction(ICharacter character, ItemSlot itemSlot)
        {
            m_CraftData = itemSlot.Item.Definition.GetDataOfType<CraftingData>();

            var durabilityProp = itemSlot.Item.GetPropertyWithId(m_DurabilityProperty);
            float dismantleEfficiency = m_CraftData.DismantleEfficiency * (durabilityProp != null ? durabilityProp.Float / 100f : 1f);

            // Add the blueprint items to the inventory
            for (int i = 0; i < m_CraftData.Blueprint.Length; i++)
            {
                int amountToAdd = Mathf.CeilToInt(m_CraftData.Blueprint[i].Amount * dismantleEfficiency);
                int addedCount = character.Inventory.AddItemsWithId(m_CraftData.Blueprint[i].Item, amountToAdd);

                // If there's no space in the inventory, drop the item
                if (addedCount < amountToAdd && character.TryGetModule<IItemDropHandler>(out var dropHandler))
                    dropHandler.DropItem(new Item(m_CraftData.Blueprint[i].Item.Def, amountToAdd - addedCount));
            }

            itemSlot.Item.StackCount--;
        }
    }
}