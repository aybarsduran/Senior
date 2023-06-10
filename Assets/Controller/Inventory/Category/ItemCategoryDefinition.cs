using IdenticalStudios.InventorySystem;
using IdenticalStudios;
using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Item Category", fileName = "(Category) ")]
    public sealed class ItemCategoryDefinition : GroupDefinition<ItemCategoryDefinition, ItemDefinition>
    {
        public ItemAction[] BaseActions => m_BaseActions;
        public override Sprite Icon => m_Icon;

        [SerializeField]
        private Sprite m_Icon;

#if UNITY_EDITOR
        [SerializeField, DataReferenceDetails(NullElementName = "Untagged")]
        private DataIdReference<ItemTagDefinition> m_DefaultTag;
#endif

        [SerializeField, ReorderableList(HasLabels = false)]
        private ItemAction[] m_BaseActions = Array.Empty<ItemAction>();


    }
}