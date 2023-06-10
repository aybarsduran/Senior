using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public abstract class PlayerDataInfoUI<T> : DataInfoUI<T> where T : class
    {
#if UNITY_EDITOR
        protected Player Player { get; private set; }
#else
        protected Player Player;
#endif


        private void Awake()
        {
            var uiManager = gameObject.GetComponentInRoot<PlayerUIManager>();

            if (uiManager == null)
            {
                Debug.LogError("No UI Manager found in the parent game objects.");
                return;
            }

            if (!uiManager.IsAttached)
                uiManager.OnAttached += OnAttached;
            else
            {
                Player = uiManager.Player;
                OnAttachment();
            }

            void OnAttached()
            {
                Player = uiManager.Player;
                uiManager.OnAttached -= OnAttached;
                OnAttachment();
            }
        }

        private void OnDestroy() => OnDetachment();

        protected virtual void OnAttachment() { }
        protected virtual void OnDetachment() { }
    }
}
