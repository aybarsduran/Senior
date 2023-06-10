using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem
{
    [CreateAssetMenu(menuName = "IdenticalStudios/ Managers/Input", fileName = "InputManager")]
    public class InputManager : Manager<InputManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() => SetInstance();

        protected override void OnInitialized()
        {
            m_EscapeInput.RegisterPerformed(RaiseEscapeCallback);
            m_ActiveContext = null;
            PushContext(m_DefaultContext);
        }

        #region Input Contexts
        [SerializeField]
        private InputContextGroup m_DefaultContext;

        private static readonly List<InputContextGroup> m_Contexts = new();
        private static InputContextBase m_ActiveContext;


        public static void PushContext(InputContextGroup context)
        {
            if (context == null)
                return;

            m_Contexts.Add(context);

            // Disable previous context and enable new.
            if (m_ActiveContext != context)
            {
                if (m_ActiveContext != null)
                    m_ActiveContext.DisableContext(context);

                context.EnableContext();
                m_ActiveContext = context;
            }
        }

        public static void PopContext(InputContextGroup context)
        {
            if (context == null)
                return;

            int index = m_Contexts.IndexOf(context);

            if (index != -1)
            {
                m_Contexts.RemoveAt(index);

                if (m_ActiveContext == context)
                {
                    var contextToEnable = m_Contexts.Count == 0 ? Instance.m_DefaultContext : m_Contexts[m_Contexts.Count - 1];

                    context.DisableContext(contextToEnable);

                    if (m_Contexts.Count == 0)
                        PushContext(contextToEnable);

                    m_ActiveContext = contextToEnable;
                    m_ActiveContext.EnableContext();
                }
            }
        }
        #endregion

        #region Escape Callbacks
        public static bool HasEscapeCallbacks => s_EscapeCallbacks.Count > 0 || s_LastEscapeCallbackRemoveFrame == Time.frameCount;

        [SerializeField]
        private InputActionReference m_EscapeInput;

        private static int s_LastEscapeCallbackRemoveFrame = -1;
        private static readonly List<UnityAction> s_EscapeCallbacks = new();


        public static void PushEscapeCallback(UnityAction action)
        {
            if (action == null)
                return;

            int index = s_EscapeCallbacks.IndexOf(action);

            if (index != -1)
                s_EscapeCallbacks.RemoveAt(index);

            s_EscapeCallbacks.Add(action);
        }

        public static void PopEscapeCallback(UnityAction action)
        {
            int index = s_EscapeCallbacks.IndexOf(action);

            if (index != -1)
                s_EscapeCallbacks.RemoveAt(index);
        }

        private void RaiseEscapeCallback(InputAction.CallbackContext context)
        {
            if (s_EscapeCallbacks.Count == 0)
                return;

            int lastCallbackIndex = s_EscapeCallbacks.Count - 1;
            var callback = s_EscapeCallbacks[lastCallbackIndex];
            s_EscapeCallbacks.RemoveAt(lastCallbackIndex);
            s_LastEscapeCallbackRemoveFrame = Time.frameCount;
            callback?.Invoke();
        }
        #endregion
    }
}
