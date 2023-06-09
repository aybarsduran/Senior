using IdenticalStudios.InventorySystem;
using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [System.Serializable]
    public sealed class ContainerTagRestriction : ContainerRestriction
    {
        public bool HasValidTags => m_ValidTags.Length > 0;
        public DataIdReference<ItemTagDefinition>[] ValidTags => m_ValidTags;

        [SerializeField]
        private DataIdReference<ItemTagDefinition>[] m_ValidTags = System.Array.Empty<DataIdReference<ItemTagDefinition>>();



        public ContainerTagRestriction(params DataIdReference<ItemTagDefinition>[] validTags)
        {
            this.m_ValidTags = validTags;
        }

        public override int GetAllowedAddAmount(IItem item, int count)
        {
            var defTag = item.Definition.Tag;

            foreach (var tag in m_ValidTags)
            {
                if (defTag.Equals(tag))
                    return count;
            }

            return 0;
        }

        public override int GetAllowedRemoveAmount(IItem item, int count) => count;
    }
}
