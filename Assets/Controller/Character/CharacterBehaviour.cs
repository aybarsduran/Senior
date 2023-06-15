using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
    /// Useful for getting access to callbacks and modules of the Parent Character.
    /// </summary>
    public abstract class CharacterBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR
        protected Character Character { get; private set; } = null;
        protected bool IsBehaviourEnabled { get; private set; } = false;
#else
        protected Character Character = null;
        protected bool IsBehaviourEnabled = false;
#endif


        private void OnEnable()
        {
            if (Character == null)
            {
                Character = gameObject.GetComponentInRoot<Character>();

                if (Character == null)
                {
                    Debug.LogError("No character found in the parent game objects.");
                    return;
                }
            }

            // If the parent character is not yet initialized, wait for the initialized event then enable this behaviour.
            if (!Character.IsInitialized)
            {
                Character.Initialized += EnableBehaviour;
            }
            // If the parent character is initialized initialize this behaviour.
            else
            {
                IsBehaviourEnabled = true;
                OnBehaviourEnabled();
            }
        }

        private void EnableBehaviour()
        {
            Character.Initialized -= EnableBehaviour;
            
            IsBehaviourEnabled = true;
            OnBehaviourEnabled();
        }

        private void OnDisable()
        {
            if (!IsBehaviourEnabled || UnityUtils.IsQuittingPlayMode)
                return;

            IsBehaviourEnabled = false;
            OnBehaviourDisabled();
        }

        /// <summary>
        /// Gets called after the parent character has been initialized. (acts similarly to the Monobehaviour "OnEnable" callback).
        /// </summary>
        protected virtual void OnBehaviourEnabled() { }

        /// <summary>
        /// Gets called when this behaviour gets disabled, only if this behaviour has been initialized. (acts similarly to the Monobehaviour "OnDisable" callback).
        /// </summary>
        protected virtual void OnBehaviourDisabled() { }

        /// <summary>
        /// <para> Returns module of specified type from the parent character. </para>
		/// Use this if you are NOT sure the parent character has this module.
        /// </summary>
        protected bool TryGetModule<T>(out T module) where T : class, ICharacterModule
        {
            return Character.TryGetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the parent character. </para>
		/// Use this if you ARE sure the parent character has this module.
        /// </summary>
        protected void GetModule<T>(out T module) where T : class, ICharacterModule
        {
            Character.GetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the parent character. </para>
		/// Use this if you ARE sure the parent character has this module.
        /// </summary>
        protected T GetModule<T>() where T : class, ICharacterModule
        {
            return Character.GetModule<T>();
        }
    }
}