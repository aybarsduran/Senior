using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
    public sealed class ContainerPropertyRestriction : ContainerRestriction
    {
        public DataIdReference<ItemPropertyDefinition>[] RequiredProperties => m_RequiredProperties;

        [SerializeField]
        private DataIdReference<ItemPropertyDefinition>[] m_RequiredProperties;


        public ContainerPropertyRestriction(params DataIdReference<ItemPropertyDefinition>[] requiredProperties)
        {
            this.m_RequiredProperties = requiredProperties;
        }

        public override int GetAllowedAddAmount(IItem item, int count)
        {
            bool isValid = true;
            var def = item.Definition;

            foreach (var property in m_RequiredProperties)
                isValid &= def.HasProperty(property);

            if (!isValid)
                return 0;

            return count;
        }

        public override int GetAllowedRemoveAmount(IItem def, int count) => count;
    }
}