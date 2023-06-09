using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class RaycastManagerUI : Singleton<RaycastManagerUI>
    {
        [SerializeField]
        private EventSystem m_EventSystem;

        [SerializeField]
        private GraphicRaycaster m_Raycaster;


        [SerializeField]
        private InputActionReference m_CursorDeltaInput;

        [SerializeField]
        private InputActionReference m_CursorPositionInput;

        private PointerEventData m_PointerEventData;
        private readonly List<RaycastResult> m_RaycastResults = new();


        protected override void OnAwake()
        {
            m_CursorDeltaInput.action.Enable();
            m_CursorPositionInput.action.Enable();
            m_PointerEventData = new PointerEventData(m_EventSystem);
        }

        public Vector2 GetCursorPosition()
        {
            var cursorPosition = m_CursorPositionInput.action.ReadValue<Vector2>();
            return cursorPosition;
        }

        public Vector2 GetCursorDelta()
        {
            var cursorPosition = m_CursorDeltaInput.action.ReadValue<Vector2>().normalized;
            return cursorPosition;
        }

        public GameObject RaycastAtCursorPosition()
        {
            var cursorPosition = GetCursorPosition();
            return Raycast(cursorPosition);
        }

        public GameObject Raycast(Vector2 position)
        {
            // Set the Pointer Event Position to that of the game object
            m_PointerEventData.position = position;

            m_RaycastResults.Clear();

            // Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, m_RaycastResults);

            return m_RaycastResults.Count > 0 ? m_RaycastResults[0].gameObject : null;
        }
    }
}