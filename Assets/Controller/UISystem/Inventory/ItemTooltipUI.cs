using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class ItemTooltipUI : PlayerUIBehaviour
    {
        [SerializeField, NotNull]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, NotNull]
        private ItemDefinitionSlotUI m_ItemInfoHandler;

        [SpaceArea]

        [SerializeField, Range(1f, 100f)]
        private float m_ShowSpeed = 10f;

        private bool m_IsActive;
        private bool m_WasDragging;
        private RectTransform m_CachedRect;
        private RectTransform m_CachedRectParent;


        protected override void OnAttachment()
        {
            m_CanvasGroup.alpha = 0f;
            m_CachedRect = (RectTransform)transform;
            m_CachedRectParent = (RectTransform)m_CachedRect.parent;

            var inspection = GetModule<IInventoryInspectManager>();
            inspection.BeforeInspectionStarted += OnInventoryInspectionStarted;
            inspection.AfterInspectionEnded += OnInventoryInspectionEnded;
        }

        protected override void OnDetachment()
        {
            var inspection = GetModule<IInventoryInspectManager>();
            inspection.AfterInspectionEnded -= OnInventoryInspectionEnded;
            inspection.AfterInspectionEnded -= OnInventoryInspectionEnded;
        }

        private void OnInventoryInspectionStarted()
        {
            UpdateManager.AddLateUpdate(UpdateTooltipVisuals);
            ItemSelectorUI.HighlightedSlotChanged += UpdateTooltipInfo;
        }

        private void OnInventoryInspectionEnded()
        {
            UpdateManager.RemoveLateUpdate(UpdateTooltipVisuals);
            ItemSelectorUI.HighlightedSlotChanged -= UpdateTooltipInfo;

            m_CanvasGroup.alpha = 0f;
            m_IsActive = false;
        }

        private void UpdateTooltipInfo(ItemSlotUI slot)
        {
            m_ItemInfoHandler.SetItem(slot);
            m_IsActive = slot != null && slot.HasItem;
        }

        private void UpdateTooltipVisuals(float deltaTime)
        {
            bool isDragging = ItemDragger.IsDragging;

            if (m_WasDragging && !isDragging)
                UpdateTooltipInfo(ItemSelectorUI.HighlightedSlot);

            UpdatePosition(RaycastManagerUI.Instance.GetCursorPosition());

            bool isActive = m_IsActive && !isDragging;
            float targetAlpha = isActive ? 1f : 0f;
            float lerpSpeed = isActive ? m_ShowSpeed : m_ShowSpeed * 1.5f;

            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, targetAlpha, deltaTime * lerpSpeed);
            m_WasDragging = isDragging;
        }

        private void UpdatePosition(Vector2 pointerPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CachedRectParent, pointerPosition, null, out Vector2 position))
                m_CachedRect.anchoredPosition = position;
        }
    }
}