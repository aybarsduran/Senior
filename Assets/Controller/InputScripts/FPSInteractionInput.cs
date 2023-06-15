using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Interaction Input")]
    public class FPSInteractionInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_InteractInput;

        private IInteractionHandler m_InteractionHandler;
        
        
        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_InteractionHandler);
        }

        protected override void OnInputEnabled()
        {
            m_InteractInput.RegisterStarted(OnInteractStart);
            m_InteractInput.RegisterCanceled(OnInteractStop);
            m_InteractionHandler.InteractionEnabled = true;
        }

        protected override void OnInputDisabled()
        {
            m_InteractInput.UnregisterStarted(OnInteractStart);
            m_InteractInput.UnregisterCanceled(OnInteractStop);
            m_InteractionHandler.InteractionEnabled = false;
        }
        #endregion

        #region Input Handling
        private void OnInteractStart(InputAction.CallbackContext obj) => m_InteractionHandler.StartInteraction();
        private void OnInteractStop(InputAction.CallbackContext obj) => m_InteractionHandler.StopInteraction();
        #endregion
    }
}
