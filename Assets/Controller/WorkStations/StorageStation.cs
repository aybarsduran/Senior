using IdenticalStudios.InventorySystem;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class StorageStation : Workstation, ISaveableComponent
    {
        public ItemContainer ItemContainer => m_ItemContainer;

        [SerializeField, Range(0, 100)]
        [Tooltip("How many slots should this storage crate have.")]
        private int m_StorageSpots;

        [SpaceArea]

        [SerializeField, ReorderableList(Foldable = true)]
        private ItemGenerator[] m_InitialItems;

        [SerializeField, Tooltip("Can a character add items to this storage.")]
        private bool m_CanAddItems = true;

        [SerializeField, Tooltip("Can a character remove items from this storage.")]
        private bool m_CanRemoveItems = true;

        private ItemContainer m_ItemContainer;


        public override IItemContainer[] GetContainers() => new IItemContainer[] { m_ItemContainer };

        private void Start()
        {
            if (m_ItemContainer == null)
                GenerateContainer();

            m_InitialItems = null;
        }

        private void GenerateContainer()
        {
            m_ItemContainer = new ItemContainer("Storage", m_StorageSpots, GetContainerRestrictions());

            foreach (var itemGenerator in m_InitialItems)
                m_ItemContainer.AddItem(itemGenerator.GenerateItem());
        }

        private ContainerRestriction[] GetContainerRestrictions()
        {
            var restrictions = new List<ContainerRestriction>();
            
            if (!m_CanAddItems)
                restrictions.Add(new ContainerAddRestriction());
            
            if (!m_CanRemoveItems)
                restrictions.Add(new ContainerRemoveRestriction());

            return restrictions.ToArray();
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_ItemContainer = members[0] as ItemContainer;
            m_ItemContainer.OnLoad();
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_ItemContainer
            };

            return members;
        }
        #endregion
    }
}