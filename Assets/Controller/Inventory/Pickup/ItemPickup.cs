using IdenticalStudios.InventorySystem;
using IdenticalStudios;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

namespace IdenticalStudios.InventorySystem
{
    /// <summary>
    /// Basic item pickup.
    /// References one item from the Database.
    /// </summary>
	[HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/interaction/interactable/demo-interactables")]
    public class ItemPickup : ItemPickupBase, ISaveableComponent
    {
        public IItem AttachedItem { get; private set; }

        [SerializeField]
        [DataReferenceDetails(HasAssetReference = true, HasIcon = true)]
        private DataIdReference<ItemDefinition> m_Item = new(0);

        [SerializeField, Range(0, 100)]

        private int m_MinCount = 1;

        [SerializeField, Range(0, 100)]

        private int m_MaxCount;


        public override void LinkWithItem(IItem item)
        {
            AttachedItem = item;

            if (AttachedItem != null)
            {
                Description = item.Definition.Description;
                Title = item.Name;
            }
        }

        public override void OnInteract(ICharacter character)
        {
            base.OnInteract(character);

            if (InteractionEnabled)
                PickUpItem(character, AttachedItem);
        }

        private void Start()
        {
            if (AttachedItem == null && !m_Item.IsNull)
                LinkWithItem(new Item(m_Item.Def, Random.Range(m_MinCount, m_MaxCount + 1)));
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            AttachedItem = members[0] as IItem;
            LinkWithItem(AttachedItem);
        }

        public object[] SaveMembers()
        {
            object[] members = {
                AttachedItem
            };

            return members;
        }
        #endregion

#if UNITY_EDITOR
        public bool IsItemValid()
        {
            return !m_Item.IsNull;
        }

        public bool IsItemStackable()
        {
            return IsItemValid() && m_Item.Def.StackSize > 1;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            m_MaxCount = Mathf.Max(m_MaxCount, m_MinCount);
        }
#endif
    }
}