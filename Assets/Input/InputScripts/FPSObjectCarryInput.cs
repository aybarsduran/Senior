using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Object Carry Input")]
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/object-carry#player-object-carry-input-behaviour")]
    public class FPSObjectCarryInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_UseInput;

        [SerializeField]
        private InputActionReference m_DropInput;

        private IWieldableCarriableHandler m_ObjectCarry;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_ObjectCarry);
        }

        protected override void OnInputEnabled()
        {
            m_UseInput.RegisterStarted(OnUseCarriedObject);
            m_DropInput.RegisterStarted(OnDropAction);
        }

        protected override void OnInputDisabled()
        {
            m_UseInput.UnregisterStarted(OnUseCarriedObject);
            m_DropInput.UnregisterStarted(OnDropAction);
        }
        #endregion

        #region Input Handling
        private void OnUseCarriedObject(InputAction.CallbackContext obj) => m_ObjectCarry.UseCarriedObject();
        private void OnDropAction(InputAction.CallbackContext obj) => m_ObjectCarry.DropCarriedObjects(1);
        #endregion
    }
}