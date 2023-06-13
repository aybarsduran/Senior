using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class ItemActionsUI : MonoBehaviour
    {
        [SerializeField]
        private ItemActionUI m_Template;

        [SerializeField, Range(1, 24)]
        private int m_TemplateCount = 5;

        private ItemActionUI[] m_ActionsUI;


        private void UpdateEnabledActions(ItemSlotUI slot)
        {
            if (slot == null || !slot.HasItem)
            {
                DisableAllUIActions();
                return;
            }

            ItemAction[] actions = slot.Item.Definition.GetAllActions();
            UpdateUIActions(slot.ItemSlot, actions);
        }

        private void UpdateUIActions(ItemSlot slot, ItemAction[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                m_ActionsUI[i].SetItemSlot(slot);
                m_ActionsUI[i].SetAction(actions[i]);
            }

            for (int i = actions.Length; i < m_ActionsUI.Length; i++)
            {
                m_ActionsUI[i].SetItemSlot(null);
                m_ActionsUI[i].SetAction(null);
            }
        }

        private void DisableAllUIActions()
        {
            for (int i = 0; i < m_ActionsUI.Length; i++)
            {
                m_ActionsUI[i].SetItemSlot(null);
                m_ActionsUI[i].SetAction(null);
            }
        }

        private void OnEnable() => ItemSelectorUI.SelectedSlotChanged += UpdateEnabledActions;
        private void OnDisable() => ItemSelectorUI.SelectedSlotChanged -= UpdateEnabledActions;

        private void Awake()
        {
            m_ActionsUI = new ItemActionUI[m_TemplateCount];
            for (int i = 0; i < m_TemplateCount; i++)
                m_ActionsUI[i] = Instantiate(m_Template, transform, false);
        }
    }
}