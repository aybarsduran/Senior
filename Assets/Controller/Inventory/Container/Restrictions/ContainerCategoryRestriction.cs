using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [System.Serializable]
    public sealed class ContainerCategoryRestriction : ContainerRestriction
    {
        public DataIdReference<ItemCategoryDefinition>[] ValidTags => m_ValidCategories;

        [SerializeField]
        private DataIdReference<ItemCategoryDefinition>[] m_ValidCategories;


        public ContainerCategoryRestriction(DataIdReference<ItemCategoryDefinition>[] validCategories)
        {
            this.m_ValidCategories = validCategories;
        }

        public override int GetAllowedAddAmount(IItem item, int count)
        {
            if (m_ValidCategories == null)
                return count;

            var def = item.Definition;
            bool isValid = false;

            foreach (var category in m_ValidCategories)
                isValid |= def.ParentGroup == category.Def;

            return isValid ? count : 0;
        }

        public override int GetAllowedRemoveAmount(IItem def, int count) => count;
    }
}
