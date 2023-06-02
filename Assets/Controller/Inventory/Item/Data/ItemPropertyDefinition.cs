using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "IdenticalStudios/Items/Item Property", fileName = "(Property) ")]
    public sealed class ItemPropertyDefinition : DataDefinition<ItemPropertyDefinition>
    {
        public override string Name
        {
            get => m_PropertyName;
            protected set => m_PropertyName = value;
        }

        public override string Description => string.Empty;
        public ItemPropertyType Type => m_PropertyType;

        [SerializeField]
        private string m_PropertyName;

        [SerializeField]
        private ItemPropertyType m_PropertyType;


    }
}