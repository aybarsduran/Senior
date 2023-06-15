using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Change Arms Input")]
    public class FPSArmsChangeInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_ChangeArmsInput;

        private IWieldableArmsHandler m_ArmsHandler;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character) => character.GetModule(out m_ArmsHandler);
        protected override void OnInputEnabled() => m_ChangeArmsInput.RegisterStarted(ToggleArmsAction);
        protected override void OnInputDisabled() => m_ChangeArmsInput.UnregisterStarted(ToggleArmsAction);
        #endregion

        #region Input Handling
        private void ToggleArmsAction(InputAction.CallbackContext obj) => m_ArmsHandler.ToggleNextArmSet();
        #endregion
    }
}
