using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Item Definition", fileName = "(Item) ")]
    public sealed class ItemDefinition : GroupMemberDefinition<ItemDefinition, ItemCategoryDefinition>
    {
        public override string Name
        {
            get => m_ItemName;
            protected set => m_ItemName = value;
        }

        public override Sprite Icon => m_Icon;
        public override string Description => m_Description;
        public ItemPickup Pickup => m_Pickup;

        public int StackSize => m_StackSize;
        public float Weight => m_Weight;

        public DataIdReference<ItemTagDefinition> Tag => m_Tag;

        [SerializeField]
        [NewLabel("Name ")]
        [Tooltip("Item name.")]
        private string m_ItemName;

        [SpaceArea]

        [SerializeField, SpritePreview]
        [Tooltip("Item Icon.")]
        private Sprite m_Icon;

        [SerializeField]
        [Tooltip("Corresponding pickup for this item, so you can actually drop it, or pick it up from the ground.")]
        private ItemPickup m_Pickup;

        [SpaceArea]

        [SerializeField, Multiline]
        [Tooltip("Item description to display in the UI.")]
        private string m_Description;

        [SerializeField, Range(0.01f, 10f)]
        [Tooltip("The inventory weight of this item.")]
        private float m_Weight = 1f;

        [SerializeField, Range(1, 1000)]
        [Tooltip("How many items of this type can be stacked in a single slot.")]
        private int m_StackSize = 1;

        [SerializeField, DataReferenceDetails(NullElementName = "Untagged")]
        private DataIdReference<ItemTagDefinition> m_Tag;

        [Help("Note: Availiable actions for this item (the base actions from the parent category are also included)")]
        [SerializeField, ReorderableList(ListStyle.Lined, HasLabels = false)]
        private ItemAction[] m_Actions;

        [Help("Note: Simple data that can be changed at runtime (not shared between item instances)")]
        [SerializeField, ReorderableList(ListStyle.Lined, HasLabels = false)]
        private ItemPropertyGenerator[] m_Properties;

        [SpaceArea]
        [Help("Note: Complex data, shared between every item instance of this type.")]
        [SerializeReference, ReferencePicker]
        [ReorderableList(ListStyle.Lined, "Data")]
        private ItemData[] m_Data;


        #region Item Tag
        public static List<ItemDefinition> GetAllItemsWithTag(ItemTagDefinition tag)
        {
            if (tag == null) return null;
            int tagId = tag.Id;
            if (tagId == 0) return null;

            List<ItemDefinition> items = new List<ItemDefinition>();

            foreach (var item in Definitions)
            {
                if (item.m_Tag == tagId)
                    items.Add(item);
            }

            return items;
        }
        #endregion

        #region Item Data
        /// <summary>
        /// Returns all of the custom data present on this item.
        /// </summary>
        public ItemData[] GetAllData() => m_Data;

        /// <summary>
        /// Tries to return an item data of type T.
        /// </summary>
        public bool TryGetDataOfType<T>(out T data) where T : ItemData
        {
            var targetType = typeof(T);

            for (int i = 0; i < m_Data.Length; i++)
            {
                if (m_Data[i].GetType() == targetType)
                {
                    data = (T)m_Data[i];
                    return true;
                }
            }

            data = null;
            return false;
        }

        /// <summary>
        /// Returns an item data of the given type (if available).
        /// </summary>
        public T GetDataOfType<T>() where T : ItemData
        {
            var targetType = typeof(T);

            for (int i = 0; i < m_Data.Length; i++)
            {
                if (m_Data[i].GetType() == targetType)
                    return (T)m_Data[i];
            }

            return null;
        }

        /// <summary>
        /// Checks if this item has an item data of type T attached.
        /// </summary>
        public bool HasDataOfType(Type type)
        {
            for (int i = 0; i < m_Data.Length; i++)
            {
                if (m_Data[i].GetType() == type)
                    return true;
            }

            return false;
        }
        #endregion

        #region Item Actions
        /// <summary>
        /// Tries to return all of the items and item actions of type T.
        /// </summary>
        public static bool GetAllItemsWithAction<T>(out List<ItemDefinition> itemList, out List<T> actionList) where T : ItemAction
        {
            var items = Definitions;
            itemList = new List<ItemDefinition>();
            actionList = new List<T>();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].TryGetItemAction<T>(out var action))
                {
                    itemList.Add(items[i]);
                    actionList.Add(action);
                }
            }

            return itemList.Count > 0;
        }

        /// <summary>
        /// Returns all of the item actions present on this item.
        /// </summary>
        public ItemAction[] GetAllActions()
        {
            if (ParentGroup.BaseActions.Length == 0)
                return m_Actions;

            if (m_Actions.Length == 0)
                return ParentGroup.BaseActions;

            var parentActions = ParentGroup.BaseActions;
            var actions = new ItemAction[m_Actions.Length + parentActions.Length];

            for (int i = 0; i < m_Actions.Length; i++)
                actions[i] = m_Actions[i];

            int offset = m_Actions.Length;
            for (int i = 0; i < parentActions.Length; i++)
                actions[i + offset] = parentActions[i];

            return actions;
        }

        /// <summary>
        /// Tries to return an item action of type T.
        /// </summary>
        public bool TryGetItemAction<T>(out T action) where T : ItemAction
        {
            var targetType = typeof(T);

            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].GetType() == targetType)
                {
                    action = (T)m_Actions[i];
                    return true;
                }
            }

            action = null;
            return false;
        }

        /// <summary>
        /// Returns an item action of the given type (if available).
        /// </summary>
        public T GetActionOfType<T>() where T : ItemAction
        {
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i] is T action)
                    return action;
            }

            var parentActions = ParentGroup.BaseActions;
            for (int i = 0; i < parentActions.Length; i++)
            {
                if (parentActions[i] is T action)
                    return action;
            }

            return null;
        }

        /// <summary>
        /// Checks if this item has an item action of type T attached.
        /// </summary>
        public bool HasActionOfType(Type type)
        {
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].GetType() == type)
                    return true;
            }

            var parentActions = ParentGroup.BaseActions;
            for (int i = 0; i < parentActions.Length; i++)
            {
                if (parentActions[i].GetType() == type)
                    return true;
            }

            return false;
        }
        #endregion

        #region Item Properties
        public static List<ItemDefinition> GetAllItemsWithProperty(ItemPropertyDefinition property)
        {
            if (property == null) return null;
            int propId = property.Id;
            if (propId == 0) return null;

            List<ItemDefinition> items = new List<ItemDefinition>();

            foreach (var item in Definitions)
            {
                if (item.HasProperty(propId))
                    items.Add(item);
            }

            return items;
        }

        public ItemPropertyGenerator[] GetAllPropertyGenerators() => m_Properties;

        public bool HasProperty(int propertyId)
        {
            foreach (var prop in m_Properties)
            {
                if (prop.Property == propertyId)
                    return true;
            }

            return false;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        /// <summary>
        /// <para> Warning: This is an editor method, don't call it at runtime.</para> 
        /// Sets the tag of this item (Internal).
        /// </summary>
        public void SetTag(DataIdReference<ItemTagDefinition> tag)
        {
            m_Tag = tag;
            EditorUtility.SetDirty(this);
        }

        ///// <summary>
        ///// <para> Warning: This is an editor method, don't call it at runtime.</para> 
        ///// Tries to add a property to this item.
        ///// </summary>
        //public bool TryAddProperty(ItemPropertyDefinition property)
        //{
        //    if (property == null)
        //        return false;

        //    if (m_Properties == null)
        //    {
        //        var newGenerator = new ItemPropertyGenerator(property);
        //        m_Properties = new ItemPropertyGenerator[] { newGenerator };
        //        return true;
        //    }
        //    else
        //    {
        //        bool canAdd = true;

        //        for (int i = 0; i < m_Properties.Length; i++)
        //        {
        //            if (m_Properties[i].Property.Def == property)
        //            {
        //                canAdd = false;
        //                break;
        //            }
        //        }

        //        if (canAdd)
        //        {
        //            var newGenerator = new ItemPropertyGenerator(property);
        //            m_Properties = m_Properties.Append(newGenerator).ToArray();
        //        }

        //        return canAdd;
        //    }
        //}

        ///// <summary>
        ///// <para> Warning: This is an editor method, don't call it at runtime.</para> 
        ///// Tries to add a property to this item.
        ///// </summary>
        //public bool TryAddAction(ItemAction action)
        //{
        //    if (action == null)
        //        return false;

        //    if (m_Actions == null)
        //    {
        //        m_Actions = new ItemAction[] { action };
        //        return true;
        //    }
        //    else if (!m_Actions.Contains(action))
        //    {
        //        m_Actions = m_Actions.Append(action).ToArray();
        //        return true;
        //    }

        //    return false;
        //}

        public override void Reset()
        {
            base.Reset();
            m_Pickup = null;
        }

        protected override void OnValidate()
        {
            ListExtensions.DistinctPreserveNull(ref m_Actions);

            base.OnValidate();
        }
#endif
        #endregion
    }
}