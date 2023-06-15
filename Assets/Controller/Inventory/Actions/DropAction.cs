using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Actions/Drop Action", fileName = "(Action) Drop")]
    public sealed class DropAction : ItemAction
    {
        [Title("Dropping")]

        [SerializeField, Range(0f, 15f)]
        private float m_DropDuration = 0f;


        /// <summary>
        /// Checks if the given item can be dropped. In the current version of the asset, items can be dropped without restrictions.
        /// </summary>
        public override bool IsViableForItem(ICharacter character, ItemSlot itemSlot) => true;
        public override float GetDuration(ICharacter character, ItemSlot itemSlot) => m_DropDuration;

        public override void PerformAction(ICharacter character, ItemSlot itemSlot)
        {
            if (character.TryGetModule(out IItemDropHandler itemDropHandler))
                itemDropHandler.DropItem(itemSlot);
        }
    }
}