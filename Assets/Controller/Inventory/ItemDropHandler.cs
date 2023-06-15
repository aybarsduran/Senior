using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios.InventorySystem
{
	public sealed class ItemDropHandler : CharacterBehaviour, IItemDropHandler
	{
		[SerializeField]
		[Tooltip("The layer mask that will be used in checking for obstacles when items are dropped.")]
		private LayerMask m_DropObstacleMask = Physics.DefaultRaycastLayers;

		[SerializeField, FormerlySerializedAs("m_ItemDropSettings")]
		[Tooltip("Position, rotation offsets etc.")]
		private ObjectDropper.DropSettings m_DropSettings;

		[SerializeField]
		private StandardSound m_DropSound;

		[Line]

		[SerializeField, AssetPreview]
		[Tooltip("The prefab used when an item that's being dropped doesn't have a pickup (e.g. clothes) or when dropping multiple items at once.")]
		private GameObject m_GenericPrefab;

		private ObjectDropper m_ItemDropper;


        protected override void OnBehaviourEnabled()
        {
			m_ItemDropper = new ObjectDropper(Character, Character.ViewTransform, m_DropObstacleMask);
		}

        public void DropItem(IItem itemToDrop, float dropDelay = 0f)
		{
			if (itemToDrop == null || !IsBehaviourEnabled)
				return;

			StartItemDrop(itemToDrop, dropDelay);

			// Remove dropped item from the inventory
			Character.Inventory.RemoveItem(itemToDrop);
		}
		
		public void DropItem(ItemSlot itemSlot, float dropDelay = 0)
		{
			if (itemSlot == null || !itemSlot.HasItem || !IsBehaviourEnabled)
				return;

			StartItemDrop(itemSlot.Item, dropDelay);

			// Remove dropped item from the slot
			itemSlot.Item = null;
		}
		
		private void StartItemDrop(IItem itemToDrop, float dropDelay)
		{
			if (dropDelay > 0.01f)
				StartCoroutine(C_DropWithDelay(itemToDrop, dropDelay));
			else
				DropItem(itemToDrop);
		}
		
		private void DropItem(IItem itemToDrop) 
		{
			if (itemToDrop == null)
				return;

			GameObject prefabToDrop;

			if (itemToDrop.StackCount == 1 && itemToDrop.Definition.Pickup != null)
				prefabToDrop = itemToDrop.Definition.Pickup.gameObject;
			else
				prefabToDrop = m_GenericPrefab;

			float dropHeightMod = Character.GetModule<IMovementController>().ActiveState == MovementStateType.Crouch ? 0.5f : 1f;
			GameObject droppedObj = m_ItemDropper.DropObject(m_DropSettings, prefabToDrop, dropHeightMod);
			Character.AudioPlayer.PlaySound(m_DropSound);

			// Link the pickup with the dropped object
			if (droppedObj.TryGetComponent(out ItemPickup itemPickup))
				itemPickup.LinkWithItem(itemToDrop);
		}
		
		private IEnumerator C_DropWithDelay(IItem itemToDrop, float dropDelay) 
		{
			yield return new WaitForSeconds(dropDelay);

			DropItem(itemToDrop);
		}
    }
}