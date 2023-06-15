using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [Serializable]
	public class ContainerGenerator
	{
		public string Name => m_Name;

		[Title("General")]

		[SerializeField]
		[Tooltip("The name of the item container.")]
		private string m_Name;

		[SerializeField, Range(1, 100),]
		[Tooltip("Number of item slots that this container has (e.g. Holster 8, Backpack 25 etc.).")]
		private int m_MaxSize = 1;

		[SerializeField, Range(-1, 100)]
		[Tooltip("The max weight that this container can hold, no item can be added if it exceeds the limit.")]
		private int m_MaxWeight = 30;

		[Title("Item Filtering")]

		[SerializeField, ReorderableList(HasLabels = false)]
		[DataReferenceDetails(HasNullElement = false)]
		[Tooltip("Only items that are tagged with the specified tag can be added.")]
		private DataIdReference<ItemTagDefinition>[] m_ValidTags;

		[SerializeField, ReorderableList(HasLabels = false)]
		[DataReferenceDetails(HasNullElement = false)]
		[Tooltip("Only items with the specified properties can be added.")]
		private DataIdReference<ItemPropertyDefinition>[] m_RequiredProperties;


		public ItemContainer GenerateContainer()
		{
			var container = new ItemContainer(
				m_Name,
				m_MaxSize,
				GetAllRestrictions()
			);

			return container;
		}

		private ContainerRestriction[] GetAllRestrictions()
		{
			var restrictions = new List<ContainerRestriction>();

			if (m_ValidTags.Length > 0)
				restrictions.Add(new ContainerTagRestriction(m_ValidTags));

			if (m_RequiredProperties.Length > 0)
				restrictions.Add(new ContainerPropertyRestriction(m_RequiredProperties));
			
			if (m_MaxWeight != -1)
				restrictions.Add(new ContainerWeightRestriction((float)m_MaxWeight / (float)m_MaxSize));

			return restrictions.ToArray();
		}
	}
}