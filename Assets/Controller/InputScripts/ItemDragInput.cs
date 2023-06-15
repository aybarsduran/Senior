using IdenticalStudios.UISystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [RequireComponent(typeof(ItemDragger))]
    public class ItemDragInput : InputBehaviour
    {
        [Title("Actions")]

        [SerializeField, NotNull]
        private ItemDragger m_DragHandler;

        [SerializeField, NotNull]
        private InputActionReference m_LeftClickInput;

        [SerializeField, NotNull]
        private InputActionReference m_SplitItemStackInput;

        private Vector2 m_PointerPositionLastFrame;
        private ItemSlotUI m_DragStartSlot;
        private bool m_IsDragging;
        private bool m_PointerMovedLastFrame;


        #region Initialization
        protected override void OnInputEnabled()
        {
            m_SplitItemStackInput.Enable();
        }

        protected override void OnInputDisabled()
        {
            m_DragStartSlot = null;
            m_IsDragging = false;

            m_SplitItemStackInput.TryDisable();
        }
        #endregion

        #region Input Handling
        protected override void TickInput()
        {
            Vector2 pointerPosition = RaycastManagerUI.Instance.GetCursorPosition();
            bool pointerMovedThisFrame = (pointerPosition - m_PointerPositionLastFrame).sqrMagnitude > 0.01f;
            m_PointerPositionLastFrame = pointerPosition;

            UpdateDragging(pointerPosition, pointerMovedThisFrame, m_PointerMovedLastFrame);

            m_PointerMovedLastFrame = pointerMovedThisFrame;
        }

        private void UpdateDragging(Vector2 pointerPosition, bool pointerMovedThisFrame, bool pointerMovedLastFrame)
        {
            if (!m_IsDragging)
            {
                if (m_LeftClickInput.action.ReadValue<float>() > 0.1f && pointerMovedThisFrame && pointerMovedLastFrame)
                {
                    m_DragStartSlot = GetSlotRaycast(out _);
                    m_IsDragging = true;

                    if (m_DragStartSlot != null)
                    {
                        bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() > 0.01f;
                        m_DragHandler.OnDragStart(m_DragStartSlot, pointerPosition, splitItemStack);
                    }
                }
            }
            else
            {
                m_DragHandler.OnDrag(pointerPosition);

                if (m_LeftClickInput.action.WasReleasedThisFrame())
                {
                    if (m_DragStartSlot != null)
                    {
                        var raycastedSlot = GetSlotRaycast(out var raycastedObject);
                        m_DragHandler.OnDragEnd(m_DragStartSlot, raycastedSlot, raycastedObject);
                    }

                    m_IsDragging = false;
                }
            }
        }

        private ItemSlotUI GetSlotRaycast(out GameObject raycastedObject)
        {
            raycastedObject = RaycastManagerUI.Instance.RaycastAtCursorPosition();

            if (raycastedObject != null)
                return raycastedObject.GetComponent<ItemSlotUI>();

            return null;
        }
        #endregion

#if UNITY_EDITOR
        private void Reset()
        {
            m_DragHandler = gameObject.GetOrAddComponent<BasicItemDragger>();
        }
#endif
    }
}
