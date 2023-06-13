using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
	public class BasicItemDragger : ItemDragger
	{
		[SerializeField, PrefabObjectOnly]
		[Tooltip("Slot template prefab that will be instantiate when an item gets dragged.")]
		private ItemSlotUI m_DragTemplate;

		[SerializeField, Range(1f, 100f)]
		private float m_DragSpeed = 15f;

		private Vector2 m_DragOffset;
		private bool m_SplitItemStack;
		private ItemSlotUI m_DraggedSlot;
		private IItem m_DraggedItem;
		private RectTransform m_DraggedItemRT;
		private RectTransform m_ParentCanvasRT;

		private IItemDropHandler m_ItemDropHandler;


		protected override void OnAttachment()
		{
			var canvas = GetComponentInParent<Canvas>();
			m_ParentCanvasRT = canvas.GetComponent<RectTransform>();

			Player.TryGetModule(out m_ItemDropHandler);
		}

        public override void CancelDrag(ItemSlotUI initialSlot)
        {
			PutItemBack(initialSlot);
        }

        public override void OnDragStart(ItemSlotUI initialSlot, Vector2 pointerPosition, bool splitItemStack = false)
		{
			if (IsDragging || !initialSlot.HasItem)
				return;

			IItem startSlotItem = initialSlot.Item;

			// Item Stack splitting
			if (splitItemStack && startSlotItem.StackCount > 1)
			{
				int initialAmount = startSlotItem.StackCount;
				int half = initialAmount / 2;
				startSlotItem.StackCount = initialAmount - half;
				m_SplitItemStack = true;

				m_DraggedItem = new Item(startSlotItem.Definition, half, startSlotItem.Properties);
			}
			else
			{
				m_DraggedItem = startSlotItem;
				m_SplitItemStack = false;
			}

			m_DraggedItemRT = GetDraggedSlot(m_DraggedItem);
			m_DraggedItemRT.gameObject.SetActive(true);
			m_DraggedItemRT.SetParent(m_ParentCanvasRT, true);

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_ParentCanvasRT, pointerPosition, null, out Vector3 worldPoint))
				m_DragOffset = initialSlot.transform.position - worldPoint;

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ParentCanvasRT, pointerPosition, null, out Vector2 localPoint))
				m_DraggedItemRT.localPosition = localPoint + (Vector2)m_ParentCanvasRT.InverseTransformVector(m_DragOffset);

			IsDragging = true;
		}

		public override void OnDrag(Vector2 pointerPosition)
		{
			if (!IsDragging)
				return;

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ParentCanvasRT, pointerPosition, null, out Vector2 localPoint))
			{
				float delta = Time.deltaTime * m_DragSpeed;
				m_DraggedItemRT.localPosition = Vector3.Lerp(m_DraggedItemRT.localPosition, localPoint + (Vector2)m_ParentCanvasRT.InverseTransformVector(m_DragOffset), delta);
				m_DragOffset = Vector2.Lerp(m_DragOffset, Vector2.zero, delta * 0.5f);
			}
		}

		public override void OnDragEnd(ItemSlotUI initialSlot, ItemSlotUI dropSlot, GameObject dropObject)
		{
            if (!IsDragging)
				return;

			// Is there a slot under our pointer?
			if (dropSlot != null)
			{
				// If dropped on the initial slot.
				if (dropSlot == initialSlot)
					PutItemBack(initialSlot);
				// See if the slot allows this type of item.
				else if (!dropSlot.HasContainer || dropSlot.Container.AllowsItem(m_DraggedItem))
				{
					bool removeOriginalItem = !m_SplitItemStack;

					// If the slot is empty...
					if (!dropSlot.HasItem)
						dropSlot.ItemSlot.Item = m_DraggedItem;
					// If the slot is not empty...
					else
					{
						IItem itemUnderPointer = dropSlot.Item;

						// Can we stack the items?
						bool canStackItems = itemUnderPointer.Id == m_DraggedItem.Id &&
											 itemUnderPointer.Definition.StackSize > 1 &&
											 itemUnderPointer.StackCount < itemUnderPointer.Definition.StackSize;

						if (canStackItems)
							StackItems(dropSlot, initialSlot);
						else
						{
							removeOriginalItem = false;
							SwapItems(dropSlot, initialSlot);
						}
					}

					if (ItemSelectorUI.SelectedSlot == initialSlot)
					{
						dropSlot.Selectable.Select();
						//dropSlot.Selectable.OnPointerEnter(null);
					}
					else
					{
						if (ItemSelectorUI.SelectedSlot != null)
							ItemSelectorUI.SelectedSlot.Selectable.Select();
					}

					if (removeOriginalItem)
						initialSlot.ItemSlot.Item = null;
				}
				else
					PutItemBack(initialSlot);
			}
			// If the player dropped it on a UI object.
			else if (dropObject != null)
				PutItemBack(initialSlot);
			// Drop the item from the inventory...
			else
				m_ItemDropHandler.DropItem(initialSlot.ItemSlot);

			m_DraggedItemRT.gameObject.SetActive(false);
			m_DraggedItem = null;
            IsDragging = false;

			// Stack the items.
			void StackItems(ItemSlotUI underPointer, ItemSlotUI initialSlot)
			{
				int added = underPointer.ItemSlot.Item.ChangeStack(m_DraggedItem.StackCount);

				// Try to add the remaining items in the parent container.
				int remainedToAdd = m_DraggedItem.StackCount - added;
				if (remainedToAdd > 0)
				{
					if (initialSlot.HasItem)
						underPointer.Container.AddItem(m_DraggedItem.Id, remainedToAdd);
					else
						initialSlot.ItemSlot.Item = new Item(m_DraggedItem.Definition, remainedToAdd, m_DraggedItem.Properties);
				}
			}

			// Swap the items because they are of different kinds / not stackable / reached maximum stack size.
			void SwapItems(ItemSlotUI underPointer, ItemSlotUI initialSlot)
			{
				if (!initialSlot.Container.AllowsItem(underPointer.Item))
				{
					PutItemBack(initialSlot);
					return;
				}

				IItem temp = underPointer.Item;
				underPointer.ItemSlot.Item = m_DraggedItem;
				initialSlot.ItemSlot.Item = temp;
			}
		}
			
		private void PutItemBack(ItemSlotUI initialSlot)
		{
			if (m_SplitItemStack && initialSlot.HasItem)
				initialSlot.Item.ChangeStack(m_DraggedItem.StackCount);
			else
				initialSlot.ItemSlot.Item = m_DraggedItem;
		}

		/// <summary>
		/// Will return a clone of the template.
		/// </summary>
		private RectTransform GetDraggedSlot(IItem item)
		{
			if (m_DraggedSlot == null)
			{
				m_DraggedSlot = Instantiate(m_DragTemplate);
				var selectable = m_DraggedSlot.Selectable;

				if (selectable.TryGetComponent<Graphic>(out var graphic))
					graphic.raycastTarget = false;

				selectable.IsSelectable = false;
			}

			m_DraggedSlot.SetItem(item);
			m_DraggedSlot.gameObject.SetActive(true);

			return m_DraggedSlot.GetComponent<RectTransform>();
		}
	}
}
