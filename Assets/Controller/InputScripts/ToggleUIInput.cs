using UnityEngine;
using UnityEngine.EventSystems;

namespace IdenticalStudios.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Toggle UI Input")]
    public class ToggleUIInput : InputBehaviour
    {
        [Title("Actions")]

        [SerializeField, NotNull]
        private BaseInputModule m_InputModule;


        protected override void OnInputEnabled() => m_InputModule.enabled = true;
        protected override void OnInputDisabled() => m_InputModule.enabled = false;

        private void Start()
        {
            if (!IsContextEnabled())
                m_InputModule.enabled = false;
        }
    }
}
