using IdenticalStudios.InventorySystem;
using System;
using UnityEngine;

namespace IdenticalStudios
{
    public class FirearmItemPickup : ItemPickup
    {
        #region Internal
        [Serializable]
        public class AttachmentItemConfigurations
        {
            [SerializeField]
            private DataIdReference<ItemPropertyDefinition> m_AttachmentTypeProperty;

            [SpaceArea]

            [SerializeField, ReorderableList]
            private AttachmentItemConfiguration[] m_Configurations;


            public void AttachToItem(IItem item)
            {
                if (item.TryGetPropertyWithId(m_AttachmentTypeProperty, out var property))
                    EnableConfigurationWithID(property.ItemId);
            }

            public void EnableConfigurationWithID(int id)
            {
                foreach (var config in m_Configurations)
                {
                    bool enable = config.Item == id;
                    config.Object.SetActive(enable);
                }
            }
        }

        [Serializable]
        public class AttachmentItemConfiguration
        {
            public DataIdReference<ItemDefinition> Item => m_Item;
            public GameObject Object => m_Object;

            [SerializeField]
            private DataIdReference<ItemDefinition> m_Item;

            [SerializeField]
            private GameObject m_Object;
        }
        #endregion

        [SerializeField, SpaceArea, ReorderableList]
        private AttachmentItemConfigurations[] m_Configurations;
        
        
        public override void LinkWithItem(IItem item)
        {
            base.LinkWithItem(item);

            foreach (var config in m_Configurations)
                config.AttachToItem(AttachedItem);
        }

#if UNITY_EDITOR
        // TODO: Take the attachment position and rotation offsets from the corresponding wieldable.
        public void CalculateAttachmentPositions()
        {

        }
#endif
    }
}