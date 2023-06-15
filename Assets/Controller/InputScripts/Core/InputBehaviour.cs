using UnityEngine;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public abstract class InputBehaviour : MonoBehaviour
    {
        [Title("Context")]
        
        [SerializeField, Help("If the active context matches this context, this input behaviour will be enabled. Leaving it null will force this input to always be enabled.")]
        private InputContext m_Context;


        protected virtual void OnInputEnabled() { }
        protected virtual void OnInputDisabled() { }
        protected virtual void TickInput() { }

        protected virtual void OnEnable()
        {
            if (m_Context == null)
            {
                OnInputEnabled();
                return;
            }
            
            m_Context.ContextEnabled += OnInputEnabled;
            m_Context.ContextDisabled += OnInputDisabled;

            if (m_Context.IsEnabled)
                OnInputEnabled();
        }

        protected virtual void OnDisable()
        {
            if (m_Context == null)
            {
                OnInputDisabled();
                return;
            }
            
            m_Context.ContextEnabled -= OnInputEnabled;
            m_Context.ContextDisabled -= OnInputDisabled;

            if (m_Context.IsEnabled)
                OnInputDisabled();
        }

        protected bool IsContextEnabled() => m_Context.IsEnabled;

        private void Update()
        {
            if (m_Context == null || m_Context.IsEnabled)
                TickInput();
        }
    }
}
