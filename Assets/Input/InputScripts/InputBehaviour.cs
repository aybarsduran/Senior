using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public abstract class InputBehaviour : MonoBehaviour
    {        
        private InputContext m_Context;

        /* OnInputEnabled() and OnInputDisabled() are called when the input is enabled or disabled,
         * respectively, while TickInput() is called every frame to process input */
        protected virtual void OnInputEnabled() { }
        protected virtual void OnInputDisabled() { }
        protected virtual void TickInput() { }

        protected virtual void OnEnable()
        {
            /* This block of code checks if m_Context is null.If it is, 
             * the OnInputEnabled() method is called and the function returns. */
            if (m_Context == null)
            {
                OnInputEnabled();
                return;
            }
            /* This block of code subscribes to the ContextEnabled and ContextDisabled events of 
             * m_Context, and calls OnInputEnabled() if m_Context is currently enabled. */
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

            /* This block of code unsubscribes from the ContextEnabled and ContextDisabled events
             * of m_Context, and calls OnInputDisabled() if m_Context is currently enabled. */
            m_Context.ContextEnabled -= OnInputEnabled;
            m_Context.ContextDisabled -= OnInputDisabled;

            if (m_Context.IsEnabled)
                OnInputDisabled();
        }

        protected bool IsContextEnabled() => m_Context.IsEnabled;
        /* This method is called every frame and calls TickInput() if m_Context is either 
         * null or enabled. This allows the inheriting class to process input every frame. */
        private void Update()
        {
            if (m_Context == null || m_Context.IsEnabled)
                TickInput();
        }
    }
}
