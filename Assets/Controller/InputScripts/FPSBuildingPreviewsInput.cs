using IdenticalStudios.BuildingSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Building Previews Input")]
    [RequireComponent(typeof(InventoryMaterialsHandler))]
    public class FPSBuildingPreviewsInput : CharacterInputBehaviour
    {
        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_AddMaterialInput;

        [SerializeField]
        private InputActionReference m_CancelPreviewInput;

        private InventoryMaterialsHandler m_InventoryMaterialsHandler;
        private IStructureDetector m_StructureDetector;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            m_InventoryMaterialsHandler = GetComponent<InventoryMaterialsHandler>();
            character.GetModule(out m_StructureDetector);
        }

        protected override void OnInputEnabled()
        {
            m_CancelPreviewInput.RegisterStarted(OnCancelPreviewStart);
            m_CancelPreviewInput.RegisterCanceled(OnCancelPreviewStop);
            m_AddMaterialInput.RegisterPerformed(OnAddMaterialAction);

            m_StructureDetector.DetectionEnabled = true;
        }

        protected override void OnInputDisabled()
        {
            m_CancelPreviewInput.UnregisterStarted(OnCancelPreviewStart);
            m_CancelPreviewInput.UnregisterCanceled(OnCancelPreviewStop);
            m_AddMaterialInput.UnregisterPerfomed(OnAddMaterialAction);

            m_StructureDetector.DetectionEnabled = false;
        }
        #endregion

        #region Input Handling
        private void OnCancelPreviewStart(InputAction.CallbackContext obj) => m_StructureDetector.StartCancellingPreview();
        private void OnCancelPreviewStop(InputAction.CallbackContext obj) => m_StructureDetector.StopCancellingPreview();

        private void OnAddMaterialAction(InputAction.CallbackContext obj)
        {
            if (m_StructureDetector.StructureInView == null)
                return;

            m_InventoryMaterialsHandler.AddMaterial(m_StructureDetector.StructureInView, false);
        }
        #endregion
    }
}
