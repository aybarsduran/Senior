using IdenticalStudios.WieldableSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Wieldables Input")]
    public class FPSWieldablesInput : CharacterInputBehaviour
    {
        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_UseInput;

        [SerializeField]
        private InputActionReference m_ReloadInput;

        [SerializeField]
        private InputActionReference m_DropInput;

        [SerializeField]
        private InputActionMode m_AimType;

        [SerializeField]
        private InputActionReference m_AimInput;

        [SerializeField]
        private InputActionReference m_SelectInput;

        [SerializeField]
        private InputActionReference m_HolsterInput;

        [SerializeField]
        private InputActionReference m_HealInput;

        [SerializeField]
        private InputActionReference m_ThrowInput;

        [SerializeField]
        private InputActionReference m_ThrowableScrollInput;

        private IWieldablesController m_Controller;
        private IWieldableSelectionHandler m_Selection;
        private IWieldableHealingHandler m_HealingHandler;
        private IWieldableThrowableHandler m_ThrowableHandler;
        private WieldableHolsterHandler m_HolsterHandler;

        private IAimInputHandler m_AimInputHandler;
        private IUseInputHandler m_UseInputHandler;
        private IReloadInputHandler m_ReloadInputHandler;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            m_Controller = character.GetModule<IWieldablesController>();
            m_Selection = character.GetModule<IWieldableSelectionHandler>();
            m_HealingHandler = character.GetModule<IWieldableHealingHandler>();
            m_ThrowableHandler = character.GetModule<IWieldableThrowableHandler>();
            m_HolsterHandler = GetComponent<WieldableHolsterHandler>();

            m_Controller.WieldableEquipStopped += OnWieldableEquipStart;
            m_Controller.WieldableHolsterStarted += OnWieldableHolsterStart;

            OnWieldableEquipStart(m_Controller.ActiveWieldable);
        }

        protected override void OnBehaviourDisabled(ICharacter character)
        {
            m_Controller.WieldableEquipStopped -= OnWieldableEquipStart;
            m_Controller.WieldableHolsterStarted -= OnWieldableHolsterStart;

            OnWieldableHolsterStart(null);
        }

        protected override void OnInputEnabled()
        {
            if (m_HolsterHandler != null)
                m_HolsterInput.RegisterStarted(OnHolsterAction);

            if (m_Selection != null)
            {
                m_SelectInput.RegisterStarted(OnSelectAction);
                m_DropInput.RegisterStarted(OnDropAction);
            }

            if (m_HealingHandler != null)
                m_HealInput.RegisterStarted(OnHealAction);

            if (m_ThrowableHandler != null)
            {
                m_ThrowInput.RegisterStarted(OnThrowAction);
                m_ThrowableScrollInput.RegisterPerformed(OnThrowableScrollAction);
            }

            m_UseInput.Enable();
            m_AimInput.Enable();
            m_ReloadInput.RegisterStarted(OnReloadAction);
        }

        protected override void OnInputDisabled()
        {
            if (m_HolsterHandler != null)
                m_HolsterInput.UnregisterStarted(OnHolsterAction);

            if (m_Selection != null)
            {
                m_SelectInput.UnregisterStarted(OnSelectAction);
                m_DropInput.UnregisterStarted(OnDropAction);
            }

            if (m_HealingHandler != null)
                m_HealInput.UnregisterStarted(OnHealAction);

            if (m_ThrowableHandler != null)
            {
                m_ThrowInput.UnregisterStarted(OnThrowAction);
                m_ThrowableScrollInput.UnregisterPerfomed(OnThrowableScrollAction);
            }

            m_UseInput.TryDisable();
            m_AimInput.TryDisable();
            m_ReloadInput.UnregisterStarted(OnReloadAction);
            m_AimInputHandler?.EndAiming();
        }

        private void OnWieldableEquipStart(IWieldable wieldable)
        {
            if (wieldable == null)
                return;

            m_AimInputHandler = wieldable as IAimInputHandler;
            m_UseInputHandler = wieldable as IUseInputHandler;
            m_ReloadInputHandler = wieldable as IReloadInputHandler;
        }

        private void OnWieldableHolsterStart(IWieldable equippedWieldable)
        {
            m_AimInputHandler = null;
            m_UseInputHandler = null;
            m_ReloadInputHandler = null;
        }
        #endregion

        #region Input Handling
        protected override void TickInput()
        {
            if (m_UseInputHandler != null)
            {
                if (m_UseInput.action.triggered)
                    m_UseInputHandler.Use(UsePhase.Start);
                else if (m_UseInput.action.ReadValue<float>() > 0.001f)
                    m_UseInputHandler.Use(UsePhase.Hold);
                else if (m_UseInput.action.WasReleasedThisFrame() || !m_UseInput.action.enabled)
                    m_UseInputHandler.Use(UsePhase.End);
            }

            if (m_AimInputHandler != null)
            {
                if (m_AimType == InputActionMode.Hold)
                {
                    if (m_AimInput.action.ReadValue<float>() > 0.001f)
                    {
                        if (!m_AimInputHandler.IsAiming)
                            m_AimInputHandler.StartAiming();
                    }
                    else if (m_AimInputHandler.IsAiming)
                        m_AimInputHandler.EndAiming();

                    return;
                }

                if (m_AimType == InputActionMode.Toggle)
                {
                    if (m_AimInput.action.WasPressedThisFrame())
                    {
                        if (m_AimInputHandler.IsAiming)
                            m_AimInputHandler.EndAiming();
                        else
                            m_AimInputHandler.StartAiming();
                    }

                    return;
                }
            }
        }

        private void OnSelectAction(InputAction.CallbackContext obj)
        {
            int index = (int)obj.ReadValue<float>() - 1;
            m_Selection.SelectAtIndex(index);
        }

        private void OnReloadAction(InputAction.CallbackContext obj) => m_ReloadInputHandler?.StartReloading();
        private void OnDropAction(InputAction.CallbackContext obj) => m_Selection.DropWieldable();
        private void OnHealAction(InputAction.CallbackContext obj) => m_HealingHandler?.TryHeal();
        private void OnHolsterAction(InputAction.CallbackContext obj) => m_HolsterHandler.Holster();
        private void OnThrowAction(InputAction.CallbackContext obj) => m_ThrowableHandler?.TryThrow();
        private void OnThrowableScrollAction(InputAction.CallbackContext obj) => m_ThrowableHandler.SelectNext(obj.ReadValue<float>() > 0);
        #endregion
    }
}