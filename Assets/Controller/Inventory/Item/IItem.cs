using UnityEngine.Events;

namespace IdenticalStudios.InventorySystem
{
    public interface IItem
    {
		ItemDefinition Definition { get; }
		IItemProperty[] Properties { get; }

		int Id { get; }
		string Name { get; }
		int StackCount { get; set; }
		float TotalWeight { get; }

		/// <summary>
		/// Raised when the stack count of this item changes.
		/// </summary>
		event UnityAction StackCountChanged;

		/// <summary>
		/// Raised when a property on this item changes.
		/// </summary>
		event UnityAction PropertyChanged;
    }
}