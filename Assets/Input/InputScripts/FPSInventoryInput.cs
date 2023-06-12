using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public class FPSInventoryInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_InventoryToggle;

        private IInventoryInspectManager m_InventoryInspection;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_InventoryInspection);
        }

        protected override void OnInputEnabled()
        {
            m_InventoryToggle.RegisterStarted(OnInventoryToggleInput);
        }

        protected override void OnInputDisabled()
        {
            m_InventoryToggle.UnregisterStarted(OnInventoryToggleInput);
        }
        #endregion

        #region Input Handling
        private void OnInventoryToggleInput(InputAction.CallbackContext context)
        {
            if (!m_InventoryInspection.IsInspecting)
                m_InventoryInspection.StartInspection(null);
            else
                m_InventoryInspection.StopInspection();
        }
        #endregion
    }
}
