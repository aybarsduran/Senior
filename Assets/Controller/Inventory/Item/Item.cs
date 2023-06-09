using IdenticalStudios.InventorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    /// <summary>
    /// Basic inventory item
    /// </summary>
    [Serializable]
    public sealed class Item : IItem, ISerializationCallbackReceiver
    {
        public ItemDefinition Definition => m_Definition;
        public IItemProperty[] Properties => m_Properties;

        public int Id => m_Id;
        public string Name => Definition.Name;

        public int StackCount
        {
            get => m_StackCount;
            set
            {
                int oldStack = m_StackCount;
                m_StackCount = Mathf.Clamp(value, 0, m_Definition.StackSize);

                if (m_StackCount == oldStack)
                    return;

                StackCountChanged?.Invoke();
            }
        }

        public float TotalWeight
        {
            get
            {
                if (m_Properties == Array.Empty<IItemProperty>())
                    return Definition.Weight * m_StackCount;

                float weight = Definition.Weight;
                foreach (var prop in m_Properties)
                {
                    if (prop.Type == ItemPropertyType.Item && prop.ItemId != 0)
                        weight += ItemDefinition.GetWithId(prop.ItemId).Weight;
                }

                return weight * m_StackCount;
            }
        }

        public event UnityAction PropertyChanged;
        public event UnityAction StackCountChanged;

        [SerializeField]
        private int m_Id;

        [SerializeField]
        private int m_StackCount;

        [SerializeReference]
        private IItemProperty[] m_Properties;

        [NonSerialized]
        private ItemDefinition m_Definition;


        /// <summary>
        /// Constructor that requires an item definition.
        /// </summary>
        public Item(ItemDefinition itemDef, int count = 1)
        {
            m_Definition = itemDef;

            if (itemDef == null)
                throw new NullReferenceException("Cannot create an item from a null item definition.");

            m_Id = itemDef.Id;
            m_StackCount = Mathf.Clamp(count, 1, m_Definition.StackSize);

            m_Properties = InstantiateProperties(itemDef.GetAllPropertyGenerators());

            // Listen to the property changed callbacks.
            foreach (var prop in m_Properties)
                prop.Changed += OnPropertyChanged;

            void OnPropertyChanged(IItemProperty property) => PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Constructor that requires an item definition and an array of custom properties.
        /// </summary>
        public Item(ItemDefinition itemDef, int count, IItemProperty[] customProperties)
        {
            m_Definition = itemDef;

            m_Id = itemDef.Id;
            m_StackCount = Mathf.Clamp(count, 1, m_Definition.StackSize);

            m_Properties = customProperties != null ?
                CloneProperties(customProperties)
                : InstantiateProperties(itemDef.GetAllPropertyGenerators());

            // Listen to the property changed callbacks.
            foreach (var property in m_Properties)
                property.Changed += OnPropertyChanged;

            void OnPropertyChanged(IItemProperty property) => PropertyChanged?.Invoke();
        }

        public override string ToString() => "Item Name: " + Name + " | Stack Size: " + m_StackCount;

        private IItemProperty[] CloneProperties(IReadOnlyList<IItemProperty> properties)
        {
            var clonedProperties = new IItemProperty[properties.Count];

            for (int i = 0; i < properties.Count; i++)
                clonedProperties[i] = properties[i].GetClone();

            return clonedProperties;
        }

        private IItemProperty[] InstantiateProperties(IReadOnlyList<ItemPropertyGenerator> propertyGenerators)
        {
            if (propertyGenerators == null)
                return Array.Empty<IItemProperty>();

            var properties = new IItemProperty[propertyGenerators.Count];

            for (int i = 0; i < propertyGenerators.Count; i++)
                properties[i] = propertyGenerators[i].GenerateItemProperty();

            return properties;
        }

        #region Save & Load
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            m_Definition = ItemDefinition.GetWithId(m_Id);
        }
        #endregion
    }
}