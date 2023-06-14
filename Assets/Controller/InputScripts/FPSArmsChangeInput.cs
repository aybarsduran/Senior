using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public class FPSArmsChangeInput : CharacterInputBehaviour
    {

        [SerializeField]
        private InputActionReference m_ChangeArmsInput;

        private IWieldableArmsHandler m_ArmsHandler;


        protected override void OnBehaviourEnabled(ICharacter character) => character.GetModule(out m_ArmsHandler);
        protected override void OnInputEnabled() => m_ChangeArmsInput.RegisterStarted(ToggleArmsAction);
        protected override void OnInputDisabled() => m_ChangeArmsInput.UnregisterStarted(ToggleArmsAction);

        private void ToggleArmsAction(InputAction.CallbackContext obj) => m_ArmsHandler.ToggleNextArmSet();
    }
}
