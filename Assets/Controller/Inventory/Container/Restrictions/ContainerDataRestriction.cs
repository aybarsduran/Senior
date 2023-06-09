using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
    public sealed class ContainerDataRestriction : ContainerRestriction
    {
        [SerializeField]
        private Type m_Type;


        public ContainerDataRestriction(Type type)
        {
            this.m_Type = type;
        }

        public override int GetAllowedAddAmount(IItem item, int count) => item.Definition.HasDataOfType(m_Type) ? count : 0;
        public override int GetAllowedRemoveAmount(IItem item, int count) => count;
    }
}
