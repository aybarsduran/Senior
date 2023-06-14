using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Survival Book Input")]
    public class FPSSurvivalBookInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_SurvivalBookInput;

        private IWieldableSurvivalBookHandler m_SurvivalBook;


        protected override void OnBehaviourEnabled(ICharacter character) => character.GetModule(out m_SurvivalBook);
        protected override void OnInputEnabled() => m_SurvivalBookInput.RegisterStarted(OnSurvivalBookAction);
        protected override void OnInputDisabled() => m_SurvivalBookInput.UnregisterStarted(OnSurvivalBookAction);

        private void OnSurvivalBookAction(InputAction.CallbackContext obj)
        {
            if (!m_SurvivalBook.InspectionActive)
                m_SurvivalBook.TryStartInspection();
            else
                m_SurvivalBook.TryStopInspection(null);
        }
    }
}
