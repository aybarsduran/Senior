using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Building Input")]
    public class FPSBuildingInput : CharacterInputBehaviour
    {
        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_PlacePreviewInput;

        [SerializeField]
        private InputActionReference m_BuildingRotateInput;

        [SerializeField]
        private InputActionReference m_BuildingCycleInput;

        private IBuildingController m_BuildingController;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_BuildingController);
        }

        protected override void OnInputEnabled()
        {
            m_BuildingCycleInput.RegisterStarted(OnCycleInput);
            m_BuildingRotateInput.RegisterPerformed(OnRotateInput);
            m_PlacePreviewInput.RegisterStarted(OnPlaceInput);
        }

        protected override void OnInputDisabled()
        {
            m_BuildingCycleInput.UnregisterStarted(OnCycleInput);
            m_BuildingRotateInput.UnregisterPerfomed(OnRotateInput);
            m_PlacePreviewInput.UnregisterStarted(OnPlaceInput);
        }
        #endregion

        #region Input Handling
        private void OnCycleInput(InputAction.CallbackContext ctx)
        {
            float value = ctx.ReadValue<float>();
            m_BuildingController.SelectNextBuildable(value > 0.1f);
        }

        private void OnPlaceInput(InputAction.CallbackContext ctx) => m_BuildingController.PlaceBuildable();
        private void OnRotateInput(InputAction.CallbackContext ctx) => m_BuildingController.RotationOffset += (ctx.ReadValue<float>() / 120f);

        #endregion
    }
}