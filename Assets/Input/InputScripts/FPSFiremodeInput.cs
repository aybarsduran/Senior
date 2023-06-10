using IdenticalStudios.InputSystem.Behaviours;
using IdenticalStudios.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public class FPSFiremodeInput : InputBehaviour
    {

        [SerializeField]
        private InputActionReference m_FiremodeInput;

        private FirearmIndexBasedAttachmentsManager m_FiremodeHandler;


        #region Initialization
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!TryGetComponent(out m_FiremodeHandler))
                Debug.Log($"No {nameof(FirearmIndexBasedAttachmentsManager)} found on this {gameObject}");
        }

        protected override void OnInputEnabled()
        {
            m_FiremodeInput.RegisterStarted(OnFiremodeChange);
        }

        protected override void OnInputDisabled()
        {
            m_FiremodeInput.UnregisterStarted(OnFiremodeChange);
        }
        #endregion

        #region Input Handling
        private void OnFiremodeChange(InputAction.CallbackContext obj) => m_FiremodeHandler.ToggleNextAttachment();
        #endregion
    }
}
