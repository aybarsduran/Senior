using IdenticalStudios.InventorySystem;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.UISystem
{
    public class ItemContainerUI : PlayerUIBehaviour
	{
		public IReadOnlyList<ItemSlotUI> Slots => m_Slots;

		public IItemContainer ItemContainer
		{
			get
			{
				if (m_IsAttached)
					return m_ItemContainer;
				else
				{
					Debug.LogError("There's no item container linked. Can't retrieve any!");

					return null;
				}
			}
		}

		public event UnityAction<IItemContainer> onContainerAttached;

		[SerializeField, ChildObjectOnly]
		private RectTransform m_SlotsRoot;

		[SerializeField, PrefabObjectOnly]
		private ItemSlotUI m_SlotTemplate;

		private IItemContainer m_ItemContainer;
		private List<ItemSlotUI> m_Slots;
		private bool m_IsAttached;


		public void AttachToContainer(IItemContainer container)
		{
			if (container == null)
			{
				Debug.Log($"Cannot attach a null container to ''{gameObject.name}''");
				return;
			}

			if (m_Slots == null || m_Slots.Count != container.Capacity)
				GenerateSlots(container.Capacity);

			m_ItemContainer = container;
			for (int i = 0; i < container.Capacity; i++)
				m_Slots[i].SetItemSlot(container[i]);

			m_IsAttached = true;
			onContainerAttached?.Invoke(m_ItemContainer);
		}

		public void DetachFromContainer()
		{
			if (m_ItemContainer == null)
				return;

			for (int i = 0; i < m_Slots.Count; i++)
				m_Slots[i].SetItemSlot(null);

			m_IsAttached = false;
		}

		public void GenerateSlots(int count)
		{
			if (count < 0 || count > 100)
				throw new System.IndexOutOfRangeException();

			// Get the children slots.
			if (m_Slots == null || !Application.isPlaying)
			{
				var slots = m_SlotsRoot.gameObject.GetComponentsInFirstChildren<ItemSlotUI>();
				m_Slots = slots;
			}

			if (count == m_Slots.Count)
				return;

			if (count < m_Slots.Count)
			{
				int slotsToDestroy = m_Slots.Count - count;
				int indexToDestroy = 0;
				while (indexToDestroy < slotsToDestroy)
				{
#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						DestroyImmediate(m_Slots[indexToDestroy].gameObject);
						indexToDestroy++;
						continue;
					}
#endif
					Destroy(m_Slots[indexToDestroy].gameObject);
					indexToDestroy++;
				}

				m_Slots.RemoveRange(0, slotsToDestroy);
				return;
			}

			if (count > m_Slots.Count)
			{
				if (m_SlotTemplate == null)
				{
					Debug.LogError("No slot template is provided, can't generate any slots.", gameObject);
					return;
				}

				int slotsToCreate = count - m_Slots.Count;

#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					EditorUtility.SetDirty(this);
					for (int i = 0; i < slotsToCreate; i++)
					{
						ItemSlotUI slotInterface = PrefabUtility.InstantiatePrefab(m_SlotTemplate, m_SlotsRoot) as ItemSlotUI;
						slotInterface.gameObject.SetActive(true);
						slotInterface.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
						EditorUtility.SetDirty(slotInterface);
					}

					return;
				}
#endif

				m_Slots.Capacity = count;
				for (int i = 0; i < slotsToCreate; i++)
				{
					ItemSlotUI slotInterface = Instantiate(m_SlotTemplate, m_SlotsRoot);
					slotInterface.gameObject.SetActive(true);
					slotInterface.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					m_Slots.Add(slotInterface);
				}
			}
		}

        protected override void OnDetachment()
        {
			DetachFromContainer();
        }

#if UNITY_EDITOR
        private void OnValidate()
		{
			if (m_SlotsRoot == null)
				m_SlotsRoot = transform as RectTransform;
		}
#endif
	}
}