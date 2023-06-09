using System;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
    public sealed class ContainerRemoveRestriction : ContainerRestriction
    {
        public override int GetAllowedAddAmount(IItem item, int count) => count;
        public override int GetAllowedRemoveAmount(IItem item, int count) => 0;
    }
}