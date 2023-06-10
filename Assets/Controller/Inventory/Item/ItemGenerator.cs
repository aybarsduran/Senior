using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IdenticalStudios.InventorySystem
{
    /// <summary>
    /// Generates an item instance based on a few parameters.
    /// </summary>
    [Serializable]
    public class ItemGenerator : ISerializationCallbackReceiver
    {
        [SerializeField]
        private ItemGenerationMethod m_Method;

        [SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<ItemDefinition> m_SpecificItem;

        [SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<ItemCategoryDefinition> m_Category;

        [SerializeField, Range(1, 100)]
        private int m_MinCount = -1;

        [SerializeField, Range(1, 100)]
        private int m_MaxCount = -1;


        public IItem GenerateItem()
        {
            switch (m_Method)
            {
                case ItemGenerationMethod.Specific:
                    return CreateItem(m_SpecificItem.Def);
                case ItemGenerationMethod.RandomFromCategory:
                {
                    var category = m_Category.Def;
                    if (category != null)
                    {
                        var itemDef = category.Members.SelectRandom();
                        if (itemDef != null)
                            return CreateItem(itemDef);
                    }
                    break;
                }
                case ItemGenerationMethod.Random:
                {
                    var category = ItemCategoryDefinition.Definitions.SelectRandom();

                    if (category != null)
                    {
                        ItemDefinition itemDef = category.Members.SelectRandom();

                        if (itemDef != null)
                            return CreateItem(itemDef);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        public ItemDefinition GetItemDefinition()
        {
            switch (m_Method)
            {
                case ItemGenerationMethod.Specific:
                    return m_SpecificItem.Def;
                case ItemGenerationMethod.RandomFromCategory:
                {
                    ItemDefinition itemDef = m_Category.Def.Members.SelectRandom();
                    return itemDef;
                }
                case ItemGenerationMethod.Random:
                {
                    var category = ItemCategoryDefinition.Definitions.SelectRandom();

                    if (category != null)
                    {
                        ItemDefinition itemDef = category.Members.SelectRandom();
                        return itemDef;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
       
        private IItem CreateItem(ItemDefinition itemDef) 
        {
            int itemCount = Random.Range(m_MinCount, m_MaxCount + 1);

            if (itemCount == 0 || itemDef == null)
                return null;

            return new Item(itemDef, itemCount);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            m_MinCount = Mathf.Max(m_MinCount, 1);
            m_MaxCount = Mathf.Max(m_MaxCount, 1);
        }
    }

    public enum ItemGenerationMethod 
    {
        Specific,
        Random,
        RandomFromCategory
    }
}