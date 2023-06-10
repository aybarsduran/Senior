using IdenticalStudios.InputSystem.Behaviours;
using IdenticalStudios;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    
    public class FPSLookInput : CharacterInputBehaviour
    {
        [SerializeField]
        private InputActionReference m_LookInput;

        private ILookHandler m_LookHandler;

        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_LookHandler);
        }

        protected override void OnInputEnabled()
        {
            //inputHelper classý olusturulcak
            m_LookHandler.SetLookInput(GetInput);
        }

        protected override void OnInputDisabled()
        {
            m_LookHandler.SetLookInput(null);
        }
        #endregion

        #region Input Handling
        private Vector2 GetInput()
        {
            Vector2 lookInput = m_LookInput.action.ReadValue<Vector2>() * 0.1f;
            lookInput.ReverseVector();

            return lookInput;
        }
        #endregion
    }
}