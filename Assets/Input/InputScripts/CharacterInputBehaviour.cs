using IdenticalStudios.InputSystem.Behaviours;
using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios.InputSystem.Behaviours
{
    public abstract class CharacterInputBehaviour : InputBehaviour
    {
#if UNITY_EDITOR
        protected Character Character { get; private set; }
#else
        protected Character Character;
#endif

        private bool m_Initialized;


        protected virtual void OnBehaviourEnabled(ICharacter character) { }
        protected virtual void OnBehaviourDisabled(ICharacter character) { }

        protected sealed override void OnEnable()
        {
            if (Character == null)
                Character = transform.root.GetComponentInChildren<Character>();

            if (Character == null)
            {
                Debug.LogError($"({gameObject.name}) No parent character found.");
                return;
            }

            if (Character.IsInitialized)
            {
                OnBehaviourEnabled(Character);
                m_Initialized = true;
                base.OnEnable();
            }
            else
                Character.Initialized += OnInitialized;
        }

        protected sealed override void OnDisable()
        {
            if (!m_Initialized || UnityUtils.IsQuittingPlayMode)
                return;

            OnBehaviourDisabled(Character);

            base.OnDisable();
        }

        private void OnInitialized()
        {
            Character.Initialized -= OnInitialized;
            OnBehaviourEnabled(Character);
            m_Initialized = true;
            base.OnEnable();
        }
    }
}
