using IdenticalStudios.InputSystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public sealed class ItemWheelUI : PlayerUIBehaviour, IWheelUI
    {
        public bool IsInspecting { get; private set; }

        [SerializeField]
        private InputContextGroup m_ItemWheelContext;

        [Title("References")]

        [SerializeField, NotNull]
        private SelectableGroupBaseUI m_SlotsGroup;

        [SerializeField, NotNull]
        private ItemSlotUI m_InfoHandler;

        [SerializeField, NotNull]
        private Transform m_SlotsRoot;

        [SerializeField, NotNull]
        private PanelUI m_Panel;

        [Title("Settings")]

        [SerializeField, Range(0.1f, 25f)]
        private float m_Range = 3f;

        [SerializeField, Range(0f, 5f)]
        private float m_Cooldown = 0.35f;

        [SerializeField, Range(0.1f, 25f)]
        private float m_Sensitivity = 3f;

        [SpaceArea]

        [SerializeField]
        private UnityEvent m_StartSelectionEvent;

        [SerializeField]
        private UnityEvent m_StopSelectionEvent;

        private float m_NextTimeCanInspect = 1f;

        [SerializeField]
        private int m_HighlightedSlot = -1;

        private Vector2 m_CursorDirection;

        private ItemWheelSlotUI[] m_WheelSlots;
        private IWieldableSelectionHandler m_SelectionHandler;


        protected override void OnAttachment()
        {
            GetModule(out m_SelectionHandler);
            m_WheelSlots = GetComponentsInChildren<ItemWheelSlotUI>();

            m_SlotsGroup.SelectedChanged += OnSelectableSelected;
        }

        protected override void OnDetachment()
        {
            m_SlotsGroup.SelectedChanged -= OnSelectableSelected;
        }

        private void OnSelectableSelected(SelectableUI selectable)
        {
            var slot = selectable != null ? selectable.GetComponent<ItemSlotUI>() : null;
            var slotIndex = slot != null ? m_WheelSlots.IndexOf(slot) : -1;

            m_HighlightedSlot = slotIndex;
            if (slotIndex == -1)
                m_InfoHandler.SetItemSlot(null);
            else
            {
                m_SelectionHandler.SelectAtIndex(slotIndex);
                m_InfoHandler.SetItemSlot(slot.ItemSlot);
            }
        }

        public void StartInspection()
        {
            if (IsInspecting || m_NextTimeCanInspect > Time.time)
                return;

            m_Panel.Show(true);

            HandleSlotHighlighting(m_SelectionHandler.SelectedIndex);
            var selectedSlot = m_WheelSlots[m_HighlightedSlot];
            selectedSlot.Selectable.Select();

            m_CursorDirection = Vector2.zero;

            InputManager.PushEscapeCallback(EndInspection);
            InputManager.PushContext(m_ItemWheelContext);
            BlurBackgroundUI.EnableBlur();

            IsInspecting = true;

            m_StartSelectionEvent.Invoke();
        }

        public void EndInspectionAndSelectHighlighted()
        {
            if (!IsInspecting)
                return;

            var highlightedSlot = m_WheelSlots[m_HighlightedSlot];
            highlightedSlot.Selectable.Select();

            EndInspection();
        }

        public void EndInspection()
        {
            if (!IsInspecting)
                return;

            var highlightedSlot = m_WheelSlots[m_HighlightedSlot];
            highlightedSlot.Selectable.OnPointerExit(null);
            m_InfoHandler.SetItem(highlightedSlot.Item);

            IsInspecting = false;
            m_NextTimeCanInspect = Time.time + m_Cooldown;
            m_Panel.Show(false);

            InputManager.PopEscapeCallback(EndInspection);
            InputManager.PopContext(m_ItemWheelContext);
            BlurBackgroundUI.DisableBlur();

            m_StopSelectionEvent.Invoke();
        }

        public void UpdateSelection(Vector2 input)
        {
            if (!IsInspecting)
                return;

            int highlightedSlot = GetHighlightedSlot(input);

            if (highlightedSlot != m_HighlightedSlot && highlightedSlot != -1)
                HandleSlotHighlighting(highlightedSlot);
        }

        private int GetHighlightedSlot(Vector2 input)
        {
            var directionOfSelection = new Vector2(input.x, input.y).normalized * m_Range;

            if (directionOfSelection != Vector2.zero) 
                m_CursorDirection = Vector2.Lerp(m_CursorDirection, directionOfSelection, Time.deltaTime * m_Sensitivity);

            float angle = -Vector2.SignedAngle(Vector2.up, m_CursorDirection);

            if (angle < 0)
                angle = 360f - Mathf.Abs(angle);

            angle = 360f - angle;

            for (int i = 0; i < m_WheelSlots.Length; i++)
            {
                Vector2 angleCoverage = m_WheelSlots[i].AngleCoverage;

                if (angleCoverage.x < angleCoverage.y)
                {
                    if (angle >= angleCoverage.x && angle <= angleCoverage.y)
                        return i;
                }
                else
                {
                    if (angle <= angleCoverage.x && angle >= angleCoverage.y)
                        return i;
                }
            }

            return -1;
        }

        private void HandleSlotHighlighting(int targetSlotIndex)
        {
            if (m_HighlightedSlot != -1)
                m_WheelSlots[m_HighlightedSlot].Selectable.OnPointerExit(null);

            m_HighlightedSlot = targetSlotIndex;

            var highlightedSlot = m_WheelSlots[targetSlotIndex];
            highlightedSlot.Selectable.OnPointerEnter(null);

            m_InfoHandler.SetItemSlot(highlightedSlot.ItemSlot);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_SlotsRoot == null)
            {
                var container = GetComponentInChildren<ItemContainerUI>();
                m_SlotsRoot = container != null ? container.transform : transform;
            }
        }
#endif
    }
}
