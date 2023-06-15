using UnityEngine;

namespace IdenticalStudios.UISystem
{
    /// <summary>
    /// Useful for getting access to callbacks and modules of the Parent Character.
    /// </summary>
    public abstract class PlayerUIBehaviour : MonoBehaviour
	{
        #region Internal
        protected enum InitMode
        {
            Start,
            Awake,
            Manually
        }
        #endregion

#if UNITY_EDITOR
        protected Player Player { get; private set;}
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

        /// <summary>
        /// Gets called when the UI gets attached to a Player
        /// </summary>
        protected virtual void OnAttachment() { }

        /// <summary>
        /// Gets called when the UI gets detached from the Player
        /// </summary>
        protected virtual void OnDetachment() { }

        /// <summary>
        /// <para> Returns module of specified type from the attached Player. </para>
        /// Use this if you are NOT sure the attached Player has this module.
        /// </summary>
        protected bool TryGetModule<T>(out T module) where T : class, ICharacterModule
        {
            return Player.TryGetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the attached Player. </para>
        /// Use this if you are sure the attached Player has this module.
        /// </summary>
        protected void GetModule<T>(out T module) where T : class, ICharacterModule
        {
            Player.GetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the attached Player. </para>
		/// Use this if you are sure the attached Player has this module.
        /// </summary>
        protected T GetModule<T>() where T : class, ICharacterModule
        {
            return Player.GetModule<T>();
        }
    }
}
