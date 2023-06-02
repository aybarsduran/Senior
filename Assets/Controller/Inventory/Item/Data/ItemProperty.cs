using IdenticalStudios.InventorySystem;
using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    // Item properties hold values that can be changed and manipulated at runtime resulting in dynamic behaviour (float, bool and integer).
    [Serializable]
    public sealed class ItemProperty : IItemProperty
    {
        public int Id => m_Id;
        public string Name => ItemPropertyDefinition.GetWithId(m_Id).Name;
        public ItemPropertyType Type => m_Type;

        public bool Boolean
        {
            get => m_Value > 0f;
            set
            {
                if (m_Type == ItemPropertyType.Boolean)
                    SetInternalValue(value ? 1 : 0);
            }
        }

        public int Integer
        {
            get => (int)m_Value;
            set
            {
                if (m_Type == ItemPropertyType.Integer)
                    SetInternalValue(value);
            }
        }

        public float Float
        {
            get => m_Value;
            set
            {
                if (m_Type == ItemPropertyType.Float)
                    SetInternalValue(value);
            }
        }

        public int ItemId
        {
            get => (int)m_Value;
            set
            {
                if (m_Type == ItemPropertyType.Item)
                    SetInternalValue(Mathf.Clamp(value, -9999999, 9999999));
            }
        }

        public event PropertyChangedCallback Changed;

        [SerializeField]
        private int m_Id;

        [SerializeField]
        private ItemPropertyType m_Type;

        [SerializeField]
        private float m_Value;


        public ItemProperty(ItemPropertyDefinition definition, float value)
        {
            m_Id = definition.Id;
            m_Type = definition.Type;
            m_Value = value;
        }

        public IItemProperty GetClone() => (IItemProperty)MemberwiseClone();

        private void SetInternalValue(float value)
        {
            float oldValue = m_Value;
            m_Value = value;

            if (Math.Abs(oldValue - m_Value) > 0.01f)
                Changed?.Invoke(this);
        }
    }
}