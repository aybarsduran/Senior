using System;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
    public sealed class ContainerAddRestriction : ContainerRestriction
    {
        public override int GetAllowedAddAmount(IItem item, int count) => 0;
        public override int GetAllowedRemoveAmount(IItem item, int count) => count;
    }
}