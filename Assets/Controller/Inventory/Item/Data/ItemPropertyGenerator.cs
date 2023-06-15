using System;
using System.Data.SqlTypes;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    using Random = UnityEngine.Random;

    /// <summary>
    /// Generates an item property instance based on a few parameters.
    /// </summary>
    [Serializable]
    public class ItemPropertyGenerator : INullable
    {
        public DataIdReference<ItemPropertyDefinition> Property => m_ItemPropertyId;
        public bool IsNull => Property.Def == null;

        [SerializeField]
        private int m_ItemPropertyId;

        [SerializeField]
        private bool m_UseRandomValue;

        [SerializeField]
        private Vector2 m_ValueRange;


        public ItemPropertyGenerator(ItemPropertyDefinition property)
        {
            this.m_ItemPropertyId = new DataIdReference<ItemPropertyDefinition>(property);
            this.m_UseRandomValue = false;
            this.m_ValueRange = Vector2.zero;
        }

        public ItemProperty GenerateItemProperty()
        {
            return new ItemProperty(Property.Def, GetValue());
        }

        public float GetValue()
        {
            return ItemPropertyDefinition.GetWithId(m_ItemPropertyId).Type switch
            {
                ItemPropertyType.Integer => m_UseRandomValue ? Random.Range((int)m_ValueRange.x, (int)m_ValueRange.y) : m_ValueRange.x,
                ItemPropertyType.Float => m_UseRandomValue ? Random.Range(m_ValueRange.x, m_ValueRange.y) : m_ValueRange.x,
                ItemPropertyType.Boolean => m_ValueRange.x,
                ItemPropertyType.Item => m_ValueRange.x,
                _ => m_ValueRange.x,
            };
        }
    }
}