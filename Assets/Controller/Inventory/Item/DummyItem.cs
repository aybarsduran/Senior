using IdenticalStudios.InventorySystem;
using System;
using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    /// <summary>
    /// Use this item if you need something that just references an item definition and doesn't implement anything else.
    /// </summary>
    public sealed class DummyItem : IItem
    {
        public ItemDefinition Definition { get; set; }
        public IItemProperty[] Properties { get; } = Array.Empty<IItemProperty>();

        public int Id => Definition != null ? Definition.Id : 0;
        public string Name => Definition != null ? Definition.Name : string.Empty;

        public int StackCount { get; set; }
        public float TotalWeight => Definition != null ? Definition.Weight * StackCount : 0f;

        public event UnityAction PropertyChanged
        {
            add { }
            remove { }
        }

        public event UnityAction StackCountChanged
        {
            add { }
            remove { }
        }
    }
}
