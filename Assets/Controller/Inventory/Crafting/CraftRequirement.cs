using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
    public struct CraftRequirement
    {
        public DataIdReference<ItemDefinition> Item;

        [Range(1, 20)]
        public int Amount;


        public CraftRequirement(int itemId, int amount)
        {
            this.Item = new DataIdReference<ItemDefinition>(itemId);
            this.Amount = amount;
        }
    }
}
