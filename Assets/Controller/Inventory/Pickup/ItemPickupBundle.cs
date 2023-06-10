using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public sealed class ItemPickupBundle : ItemPickupBase, ISaveableComponent
    {
		[SerializeField, ReorderableList]
		private ItemGenerator[] m_Items = new ItemGenerator[1];

		private List<IItem> m_AttachedItems;


		public override void LinkWithItem(IItem item)
		{
			if (m_AttachedItems == null)
				m_AttachedItems = new List<IItem>();

			m_AttachedItems.Add(item);
		}

		public override void OnInteract(ICharacter character)
		{
			base.OnInteract(character);

			if (InteractionEnabled)
			{
                for (int i = 0; i < m_AttachedItems.Count; i++)
                    PickUpItem(character, m_AttachedItems[i], i == 0);
            }
		}

		private void Start()
		{
			if (m_AttachedItems == null)
			{
				for (int i = 0; i < m_Items.Length; i++)
					LinkWithItem(m_Items[i].GenerateItem());
			}
		
			var stringBuilder = new StringBuilder(m_AttachedItems.Count * 10);

            for (int i = 0; i < m_AttachedItems.Count; i++)
            {
                IItem item = m_AttachedItems[i];
                stringBuilder.Append($"''{item.Name}'' x {item.StackCount}\n");
            }

            Description = stringBuilder.ToString();
		}

		#region Save & Load
		public void LoadMembers(object[] members)
		{
			m_AttachedItems = members[0] as List<IItem>;
		}

		public object[] SaveMembers()
		{
			object[] members = {
				m_AttachedItems
			};

			return members;
		}
        #endregion
    }
}
