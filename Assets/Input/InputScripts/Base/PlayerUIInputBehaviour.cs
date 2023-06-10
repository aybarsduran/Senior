using IdenticalStudios.UISystem;
using UnityEngine;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public abstract class PlayerUIInputBehaviour : InputBehaviour
    {
#if UNITY_EDITOR
        protected Player Player { get; private set; }
#else
        protected Player Player;
#endif

        private bool m_Initialized;


        protected abstract void OnInitialized(Player character);

        protected sealed override void OnEnable()
        {
            if (Player != null)
            {
                base.OnEnable();
                return;
            }

            var UIManager = gameObject.GetComponentInRoot<PlayerUIManager>();

            if (UIManager == null)
            {
                Debug.LogError(
                    $"({gameObject.name}) No parent UI Manager found.");

                return;
            }

            if (UIManager.IsAttached)
            {
                Player = UIManager.Player;
                OnInitialized(Player);
                base.OnEnable();
            }
            else
                UIManager.OnAttached += OnPlayerInitialized;

            void OnPlayerInitialized()
            {
                UIManager.OnAttached -= OnPlayerInitialized;
                Player = UIManager.Player;
                OnInitialized(Player);
                m_Initialized = true;

                base.OnEnable();
            }
        }

        protected override void OnDisable()
        {
            if (!m_Initialized || UnityUtils.IsQuittingPlayMode)
                return;

            base.OnDisable();
        }
    }
}
