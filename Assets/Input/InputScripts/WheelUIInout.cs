using IdenticalStudios.UISystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [RequireComponent(typeof(IWheelUI))]
    public class WheelUIInput : InputBehaviour
    {

        [SerializeField]
        private InputActionReference m_ItemWheelInput;

        private IWheelUI m_Wheel;


        #region Initialization
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Wheel = GetComponent<IWheelUI>();
        }

        protected override void OnInputEnabled()
        {
            m_ItemWheelInput.RegisterPerformed(OnWheelOpen);
            m_ItemWheelInput.RegisterCanceled(OnWheelCloseInput);
        }

        protected override void OnInputDisabled()
        {
            m_ItemWheelInput.UnregisterPerfomed(OnWheelOpen);
            m_ItemWheelInput.UnregisterCanceled(OnWheelCloseInput);
        }
        #endregion

        #region Input handling
        protected override void TickInput()
        {
            if (!m_Wheel.IsInspecting)
                return;

            m_Wheel.UpdateSelection(RaycastManagerUI.Instance.GetCursorDelta());
        }

        private void OnWheelOpen(InputAction.CallbackContext ctx) => m_Wheel.StartInspection();
        private void OnWheelCloseInput(InputAction.CallbackContext ctx) => m_Wheel.EndInspectionAndSelectHighlighted();
        #endregion
    }
}