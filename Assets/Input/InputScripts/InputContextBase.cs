using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.InputSystem
{
    public abstract class InputContextBase : ScriptableObject
    {
        public bool IsEnabled
        {
            get => m_IsEnabled;
            private set => m_IsEnabled = value;
        }

        public event UnityAction ContextEnabled;
        public event UnityAction ContextDisabled;

        [System.NonSerialized]
        private bool m_IsEnabled;


        internal virtual void EnableContext()
        {
            if (IsEnabled)
                return;

            IsEnabled = true;
            ContextEnabled?.Invoke();
        }

        internal virtual void DisableContext(InputContextBase newContext)
        {
            if (!IsEnabled)
                return;

            IsEnabled = false;
            ContextDisabled?.Invoke();
        }

        internal abstract InputContextBase[] GetSubContexts();

        internal bool ContainsSubContext(InputContextBase context)
        {
            if (this == context)
                return true;

            var subContexts = GetSubContexts();
            foreach (var ctx in subContexts)
            {
                if (ctx == context)
                    return true;
            }

            return false;
        }
    }
}
