using IdenticalStudios.WieldableSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Firemode Input")]
    public class FPSFiremodeInput : InputBehaviour
    {
        [Title("Actions")]

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
