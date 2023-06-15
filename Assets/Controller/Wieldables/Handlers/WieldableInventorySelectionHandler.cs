using IdenticalStudios.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.WieldableSystem
{
    /// <summary>
    /// Takes care of selecting wieldables based on inventory items.
    /// </summary>
    [RequireComponent(typeof(IWieldablesController))]
    public class WieldableInventorySelectionHandler : CharacterBehaviour, IWieldableSelectionHandler, ISaveableComponent
    {
        #region Internal
        private enum GetWieldablesMode
        {
            SpawnFromList,
            GetFromList
        }
        #endregion

        public int SelectedIndex => Mathf.Max(m_SelectedIndex, 0);
        public int PreviousIndex => m_PrevIndex;

        public ItemSlot ItemSlot => m_Holster[m_SelectedIndex];

        public event UnityAction<int> SelectedChanged;

        [SerializeField]
        [Help("Found in the Inventory Module.")]
        [Tooltip("The corresponding inventory container (e.g. holster, backpack etc.) that this behaviour will use for selecting items.")]
        private string m_HolsterContainer = "Holster";

        [SerializeField, Range(1, 6)]
        private int m_StartingSlot = 1;

        [Title("Wieldables")]

        [SerializeField, ReorderableList(HasLabels = false)]
#if UNITY_EDITOR
        [EditorButton(nameof(LoadAllWieldables), "Find all Wieldables (templates excluded)")]
#endif
        private List<WieldableItem> m_WieldablesList;

        private int m_SelectedIndex = -1;
        private int m_PrevIndex = -1;
        private IItemContainer m_Holster;

        private Dictionary<int, IWieldableItem> m_Wieldables;

        private IWieldablesController m_Controller;
        private IItemDropHandler m_DropHandler;
        private IInventory m_Inventory;

        private IItem m_QueuedItem;
        private bool m_HasQueuedItem;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Controller);
            GetModule(out m_Inventory);
            GetModule(out m_DropHandler);

            if (m_Wieldables == null)
            {
                InitializeWieldables();
                StartCoroutine(C_SelectItemAtStartDelayed());
            }
        }

        public void SelectAtIndex(int indexToSelect, float holsterPreviousSpeed = 1f)
        {
            m_SelectedIndex = Mathf.Clamp(indexToSelect, 0, m_Holster.Capacity - 1);
            EquipWieldable(m_Holster[m_SelectedIndex].Item, holsterPreviousSpeed);

            if (m_SelectedIndex != m_PrevIndex)
                SelectedChanged?.Invoke(m_SelectedIndex);

            m_PrevIndex = m_SelectedIndex;
        }

        public IWieldableItem GetWieldableItemWithId(int id)
        {
            if (m_Wieldables.TryGetValue(id, out IWieldableItem wieldableItem))
                return wieldableItem;

            return null;
        }

        public bool HasWieldableWithId(int id) => m_Wieldables.ContainsKey(id);

        public void DropWieldable(float dropDelay = 0.35f, bool forceDrop = false)
        {
            if (!forceDrop && m_Controller.IsEquipping)
                return;

            var itemToDrop = m_Holster[m_SelectedIndex].Item;
            if (itemToDrop != null && m_Wieldables.TryGetValue(itemToDrop.Id, out var wieldable))
            {
                // Drop inventory wieldable.
                if (wieldable.Wieldable == m_Controller.ActiveWieldable)
                    m_DropHandler.DropItem(m_Holster[m_SelectedIndex], dropDelay);
            }
        }

        private void EquipWieldable(IItem itemToAttach, float holsterPrevSpeedMod = 1f)
        {
            if (itemToAttach != null && m_Wieldables.TryGetValue(itemToAttach.Id, out var wieldableItem))
            {
                // If the player tries to equip the same item, return.
                var prevSlot = m_PrevIndex != -1 ? m_Holster[m_PrevIndex] : null;
                if (prevSlot != null && prevSlot.Item == itemToAttach && wieldableItem.AttachedItem != null)
                    return;

                if (!m_Controller.TryEquipWieldable(wieldableItem.Wieldable, holsterPrevSpeedMod, () => wieldableItem.SetItem(itemToAttach)))
                {
                    m_QueuedItem = itemToAttach;
                    m_HasQueuedItem = true;
                }
            }
            else
                m_Controller.TryEquipWieldable(null, holsterPrevSpeedMod);
        }

        private void OnWieldableEquipStopped(IWieldable equippedWieldable)
        {
            if (!m_HasQueuedItem)
                return;

            m_HasQueuedItem = false;
            EquipWieldable(m_QueuedItem);
        }

        private void OnHolsterChanged(ItemSlot.CallbackContext context)
        {
            // get index of slot
            int indexOfSlot = m_Holster.GetSlotIndex(context.Slot);

            if (context.Slot.HasItem || indexOfSlot == m_SelectedIndex)
                SelectAtIndex(indexOfSlot, context.Slot.HasItem ? 1f : 1.35f);
        }

        private void InitializeWieldables()
        {
            m_Wieldables = new Dictionary<int, IWieldableItem>();

            foreach (var wieldableItem in m_WieldablesList)
            {
                if (wieldableItem == null || wieldableItem.Wieldable == null || wieldableItem.DefaultItemDefinition.IsNull)
                {
                    Debug.LogWarning("Wieldable Item or Wieldable is null.");
                    continue;
                }

                if (!m_Wieldables.ContainsKey(wieldableItem.DefaultItemDefinition))
                {
                    IWieldable wieldable;

                    if (wieldableItem.GetInstanceID() < 0)
                        wieldable = m_Controller.AddWieldable(wieldableItem.Wieldable, false);
                    else
                        wieldable = m_Controller.AddWieldable(wieldableItem.Wieldable, true);

                    if (wieldable != null)
                        m_Wieldables.Add(wieldableItem.DefaultItemDefinition, wieldable.GetComponent<IWieldableItem>());
                }
                else
                    Debug.LogError("You're trying to spawn a wieldable with an id that has already been added.");
            }
        }

        private IEnumerator C_SelectItemAtStartDelayed()
        {
            yield return null;
            yield return null;

            if (m_SelectedIndex == -1)
                m_SelectedIndex = m_StartingSlot;

            m_Holster = m_Inventory.GetContainerWithName(m_HolsterContainer);
            m_Holster.SlotChanged += OnHolsterChanged;
            m_Controller.WieldableEquipStopped += OnWieldableEquipStopped;

            if (!m_Controller.IsEquipping && m_Controller.ActiveWieldable == null)
                SelectAtIndex(m_SelectedIndex);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_SelectedIndex = (int)members[0];
            m_PrevIndex = (int)members[1];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_SelectedIndex,
                m_PrevIndex
            };

            return members;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        protected void LoadAllWieldables()
        {
            var allPrefabs = AssetDatabase.FindAssets($"t: prefab");

            var wieldables = new List<WieldableItem>();
            foreach (var guid in allPrefabs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (!obj.name.Contains("Template", System.StringComparison.OrdinalIgnoreCase) && obj.TryGetComponent(out WieldableItem wItem))
                    wieldables.Add(wItem);
            }

            m_WieldablesList = wieldables;
            EditorUtility.SetDirty(this);
        }
#endif
        #endregion
    }
}