using IdenticalStudios.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class ItemActionUI : PlayerUIBehaviour
    {
        [SerializeField]
        protected ItemAction m_ItemAction;

        [SerializeField, NotNull]
        protected Image m_IconImage;

        [SerializeField, NotNull]
        protected TextMeshProUGUI m_NameText;

        protected bool m_IsPerformingAction;
        protected ItemSlot m_ItemSlot;


        public void SetItemSlot(ItemSlot itemSlot) => m_ItemSlot = itemSlot;

        public void SetAction(ItemAction itemAction)
        {
            m_ItemAction = itemAction;

            if (m_ItemAction != null)
            {
                m_IconImage.sprite = m_ItemAction.GetDisplayIcon();
                m_NameText.text = m_ItemAction.GetDisplayName();
            }

            gameObject.SetActive(GetEnabledState());
        }

        public void StartAction()
        {
            if (m_ItemSlot == null) return;

            m_ItemAction.StartAction(Player, m_ItemSlot);
            m_IsPerformingAction = true;

            if (GetDuration() > 0.01f)
            {
                string actionName = m_NameText.text;
                string actionVerb = m_ItemAction.GetDisplayVerb();
                float duration = m_ItemAction.GetDuration(Player, m_ItemSlot);

                var aParams = new CustomActionManagerUI.AParams(actionName, actionVerb + "...", duration, true,
                    completedCallback: PerformAction,
                    cancelledCallback: CancelAction);

                CustomActionManagerUI.TryStartAction(aParams);
            }
            else
            {
                PerformAction();
            }
        }

        public void PerformAction()
        {
            if (m_ItemSlot == null) return;

            m_ItemAction.PerformAction(Player, m_ItemSlot);
            m_IsPerformingAction = false;
        }

        public void CancelAction()
        {
            if (m_ItemSlot != null && m_IsPerformingAction)
                m_ItemAction.CancelAction(Player, m_ItemSlot);
        }

        public float GetDuration()
        {
            if (m_ItemSlot == null) return 0f;

            float duration = m_ItemAction.GetDuration(Player, m_ItemSlot);
            return duration;
        }

        protected virtual bool GetEnabledState()
        {
            bool isSlotViable = m_ItemSlot != null && m_ItemSlot.HasItem;

            if (!isSlotViable)
                return false;

            bool isActionViable = m_ItemAction != null && m_ItemAction.IsViableForItem(Player, m_ItemSlot);

            return isActionViable;
        }

        protected override void OnAttachment()
        {
            if (m_ItemAction != null)
                SetAction(m_ItemAction);

            gameObject.SetActive(false);
        }
    }
}