using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public abstract class ItemDragger : PlayerUIBehaviour
    {
		public static bool IsDragging { get; protected set; }


		public abstract void OnDragStart(ItemSlotUI startSlot, Vector2 pointerPosition, bool splitItemStack = false);
		public abstract void CancelDrag(ItemSlotUI initialSlot);
		public abstract void OnDrag(Vector2 pointerPosition);
		public abstract void OnDragEnd(ItemSlotUI startSlot, ItemSlotUI dropSlot, GameObject dropObject);
	}
}
