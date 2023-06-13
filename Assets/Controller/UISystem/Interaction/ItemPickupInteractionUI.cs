using IdenticalStudios.InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class ItemPickupInteractionUI : PlayerUIBehaviour, IInteractableInfoDisplayer
    {
        public IEnumerable<Type> HoverableTypes => m_InteractableTypes;
        private static readonly Type[] m_InteractableTypes = typeof(ItemPickup).GetAllChildClasses().Append(typeof(ItemPickup)).ToArray();

        [SerializeField]
        [Tooltip("The main rect transform that will move the interactable's center (should be parent of everything else).")]
        private RectTransform m_RectTransform;

        [SerializeField]
        [Tooltip("The canvas group used to fade the item pickup displayer in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        [Tooltip("An image that used in showing the time the current interactable has been interacted with.")]
        private Image m_InteractProgressImg;

        [SerializeField]
        [Tooltip("An offset that will be applied to the position of the 'rect transform'")]
        private Vector3 m_CustomItemOffset;

        [SpaceArea]

        [SerializeField, NotNull]
        private ItemDefinitionSlotUI m_ItemInfoHandler;

        private Camera m_PlayerCamera;
        private ItemPickup m_ItemPickup;
        private bool m_IsVisible;

        private Bounds m_ItemPickupBounds;


        public void ShowInfo(IHoverable hoverable)
        {
            m_IsVisible = true;

            if (m_CanvasGroup.alpha < 0.01f)
                UpdateManager.AddUpdate(OnUpdate);
        }

        public void HideInfo() => m_IsVisible = false;

        public void UpdateInfo(IHoverable hoverable)
        {
            m_ItemPickup = hoverable as ItemPickup;
            m_IsVisible = m_ItemPickup != null && m_ItemPickup.AttachedItem != null;

            if (m_IsVisible)
            {
                m_ItemInfoHandler.SetItem(m_ItemPickup.AttachedItem);
                m_ItemPickupBounds = m_ItemPickup.GetComponentInChildren<Renderer>().bounds;
            }
        }

        public void SetInteractionProgress(float progress) => m_InteractProgressImg.fillAmount = progress;

        protected override void OnAttachment()
        {
            m_PlayerCamera = Player.GetModule<ICameraFOVHandler>().UnityWorldCamera;
            m_InteractProgressImg.fillAmount = 0f;

            m_CanvasGroup.alpha = 0f;
        }

        private void OnUpdate()
        {
            if (Player == null)
            {
                UpdateManager.RemoveUpdate(OnUpdate);
                return;
            }

            float boundsMedian = GetMedian(m_ItemPickupBounds.extents.x, m_ItemPickupBounds.extents.y, m_ItemPickupBounds.extents.z);
            Vector3 screenPosition = m_PlayerCamera.WorldToScreenPoint(m_ItemPickupBounds.center + Player.transform.right * boundsMedian);
            PositionAtScreenPoint(m_RectTransform, screenPosition + m_CustomItemOffset);

            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, (m_IsVisible ? 1f : 0f), Time.deltaTime * 20f);

            if (!m_IsVisible && m_CanvasGroup.alpha < 0.01f)
            {
                m_CanvasGroup.alpha = 0f;
                UpdateManager.RemoveUpdate(OnUpdate);
            }
        }

        private static float GetMedian(params float[] values)
        {
            float sum = 0f;

            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }

        private void PositionAtScreenPoint(RectTransform rectTransform, Vector2 screenPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, screenPosition, null, out Vector2 position))
                rectTransform.anchoredPosition = position;
        }
    }
}