using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Item Property", fileName = "(Property) ")]
    public sealed class ItemPropertyDefinition : DataDefinition<ItemPropertyDefinition>
    {
		public override string Name
		{
			get => m_PropertyName;
			protected set => m_PropertyName = value;
		}

#if UNITY_EDITOR
        public override string Description => m_Description;
#else
        public override string Description => string.Empty;
#endif
        public ItemPropertyType Type => m_PropertyType;

        [SerializeField, BeginHorizontal, HideLabel]
        private string m_PropertyName;

        [SerializeField, EndHorizontal, HideLabel]
		private ItemPropertyType m_PropertyType;

#if UNITY_EDITOR
        [SerializeField, Multiline(6)]
        [Tooltip("Property description, only shown in the editor")]
        private string m_Description;
#endif
    }
}