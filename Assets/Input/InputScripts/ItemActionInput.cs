using IdenticalStudios.InventorySystem;
using IdenticalStudios.UISystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public class ItemActionInput : PlayerUIInputBehaviour
    {
        [Title("Settings")]

        [SerializeField]
        private InputActionReference m_InputAction;

        [SerializeField]
        private ItemAction m_ItemAction;


        protected override void OnInitialized(Player player) { }
        protected override void OnInputEnabled() => m_InputAction.RegisterPerformed(OnAction);
        protected override void OnInputDisabled() => m_InputAction.UnregisterPerfomed(OnAction);

        private void OnAction(InputAction.CallbackContext context)
        {
            var raycastObject = RaycastManagerUI.Instance.RaycastAtCursorPosition();

            if (raycastObject != null && raycastObject.TryGetComponent<ItemSlotUI>(out var slotUI))
            {
                var slot = slotUI.ItemSlot;
                m_ItemAction.StartAction(Player, slot);
                m_ItemAction.PerformAction(Player, slot);
            }
        }
    }
}
