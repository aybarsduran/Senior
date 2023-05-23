using UnityEngine;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public abstract class CharacterInputBehaviour : InputBehaviour
    {
        protected Character Character;
        private bool m_Initialized;

        // Called when this behaviour is enabled for the given character
        protected virtual void OnBehaviourEnabled(ICharacter character) { }
        // Called when this behaviour is disabled for the given character
        protected virtual void OnBehaviourDisabled(ICharacter character) { }

        // Called when this behaviour is enabled
        protected sealed override void OnEnable()
        {
            if (Character == null)
                Character = transform.root.GetComponentInChildren<Character>();

            if (Character == null)
            {
                Debug.LogError($"({gameObject.name}) No parent character found.");
                return;
            }
            // If the character is already initialized, enable this behaviour
            if (Character.IsInitialized)
            {
                OnBehaviourEnabled(Character);
                m_Initialized = true;
                base.OnEnable();
            }
            // If the character is not yet initialized, wait for initialization
            else
                Character.Initialized += OnInitialized;
        }

        // Called when this behaviour is disabled
        // If this behaviour hasn't been initialized or the application is quitting, return
        protected sealed override void OnDisable()
        {
            if (!m_Initialized || UnityUtils.IsQuittingPlayMode)
                return;

            OnBehaviourDisabled(Character);

            base.OnDisable();
        }

        // Called when the character is initialized
        private void OnInitialized()
        {
            Character.Initialized -= OnInitialized;
            OnBehaviourEnabled(Character);
            m_Initialized = true;
            base.OnEnable();
        }
    }
}
