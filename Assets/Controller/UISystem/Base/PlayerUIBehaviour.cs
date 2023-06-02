using UnityEngine;

namespace IdenticalStudios.UISystem
{
    // Useful for getting access to callbacks and modules of the Parent Character.
    public abstract class PlayerUIBehaviour : MonoBehaviour
    {
        protected enum InitMode
        {
            Start,
            Awake,
            Manually
        }

#if UNITY_EDITOR
        protected Player Player { get; private set; }
#else
        protected Player Player;
#endif


        protected void InitializeUIBehaviour()
        {
            if (Player != null)
            {
                Debug.LogError("You're trying to initialize a behaviour that has already been initialized.");
                return;
            }

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

        private void Start()
        {
            if (GetInitMode() == InitMode.Start)
                InitializeUIBehaviour();
        }

        private void Awake()
        {
            if (GetInitMode() == InitMode.Awake)
                InitializeUIBehaviour();
        }

        private void OnDestroy()
        {
            OnDetachment();
        }

        protected virtual InitMode GetInitMode() => InitMode.Start;

        // Gets called when the UI gets attached to a Player
        protected virtual void OnAttachment() { }

        // Gets called when the UI gets detached from the Player
        protected virtual void OnDetachment() { }

        // Returns module of specified type from the attached Player.
        // Use this if you are NOT sure the attached Player has this module.
        protected bool TryGetModule<T>(out T module) where T : class, ICharacterModule
        {
            return Player.TryGetModule(out module);
        }

        //  Returns module of specified type from the attached Player. 
        // Use this if you are sure the attached Player has this module.
        protected void GetModule<T>(out T module) where T : class, ICharacterModule
        {
            Player.GetModule(out module);
        }

        //  Returns module of specified type from the attached Player.
		// Use this if you are sure the attached Player has this module.
        protected T GetModule<T>() where T : class, ICharacterModule
        {
            return Player.GetModule<T>();
        }
    }
}
