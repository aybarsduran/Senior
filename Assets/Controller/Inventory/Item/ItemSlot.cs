using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    /// <summary>
    /// Holds a reference to an item and listens to changes made to it.
    /// </summary>
    [Serializable]
	public sealed class ItemSlot
	{
		public struct CallbackContext
		{
			public readonly ItemSlot Slot;
			public readonly CallbackType Type;


			public CallbackContext(ItemSlot slot, CallbackType type)
			{
				this.Slot = slot;
				this.Type = type;
			}
        }
		
		public enum CallbackType
		{
			ItemAdded,
			ItemRemoved,
			StackChanged,
			PropertyChanged
		}
		
		public bool HasItem => m_Item != null;

		public IItem Item
		{
			get => m_Item;
			set
			{
				if (m_Item == value)
					return;
				
				// Stop listening for changes to the previously attached item.
				if (m_Item != null)
				{
					m_Item.PropertyChanged -= OnPropertyChanged;
					m_Item.StackCountChanged -= OnStackChanged;
				}

				m_Item = value;

				// Start listening for changes to the newly attached item.
				if (m_Item != null)
				{
					m_Item.PropertyChanged += OnPropertyChanged;
					m_Item.StackCountChanged += OnStackChanged;

					ItemChanged?.Invoke(new CallbackContext(this, CallbackType.ItemAdded));
				}
				else
					ItemChanged?.Invoke(new CallbackContext(this, CallbackType.ItemRemoved));
				
				void OnPropertyChanged() => ItemChanged?.Invoke(new CallbackContext(this, CallbackType.PropertyChanged));

				void OnStackChanged()
				{
					if (m_Item.StackCount == 0)
					{
						Item = null;
						return;
					}

					ItemChanged?.Invoke(new CallbackContext(this, CallbackType.StackChanged));
				}
			}
		}

		public bool HasContainer => m_Container != null;

		public IItemContainer Container
		{
			get
			{
				if (m_Container == null)
					Debug.LogError("This slot does not have a parent container.");

				return m_Container;
			}
		}

		/// <summary> Sent when this slot has changed (e.g. when the attached item has changed).</summary>
		public event ItemSlotChangedDelegate ItemChanged;
		
		[OdinSerializer.OdinSerialize]
		private IItem m_Item;

		private readonly IItemContainer m_Container;


		public ItemSlot(IItemContainer container)
		{
			this.m_Item = null;
			this.m_Container = container;
		}

		public ItemSlot(IItem item, IItemContainer container)
		{
			this.m_Item = item;
			this.m_Container = container;
		}
	}

	public delegate void ItemSlotChangedDelegate(ItemSlot.CallbackContext context);
}
