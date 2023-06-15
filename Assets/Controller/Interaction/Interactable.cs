using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IdenticalStudios
{
    /// <summary>
    /// Base class for interactable objects (eg. Storage boxes, doors, item pickups).
    /// Has numerous hover and interaction callbacks (overridable).
    /// </summary>
    public class Interactable : MonoBehaviour, IInteractable, IHoverable
    {
        #region Internal
        [System.Serializable]
        public class InteractEvent : UnityEvent<ICharacter> { }
        #endregion

        public float HoldDuration => m_HoldDuration;
        public bool IsHoverable => m_InteractionEnabled;

        public virtual bool InteractionEnabled
        {
            get => m_InteractionEnabled;
            set
            {
                if (value == m_InteractionEnabled)
                    return;
                
                m_InteractionEnabled = value;
                InteractionEnabledChanged?.Invoke();
            }
        }

        public string Title
        {
            get => m_InteractTitle;
            protected set => m_InteractTitle = value;
        }

        public string Description
        {
            get => m_InteractDescription;
            protected set
            {
                m_InteractDescription = value;
                DescriptionChanged?.Invoke();
            }
        }

        public event UnityAction<ICharacter> Interacted
        {
            add => m_OnInteract.AddListener(value);
            remove => m_OnInteract.RemoveListener(value);
        }

        public event UnityAction<ICharacter> HoverStarted
        {
            add => m_OnHoverStart.AddListener(value);
            remove => m_OnHoverStart.RemoveListener(value);
        }

        public event UnityAction<ICharacter> HoverEnded
        {
            add => m_OnHoverEnd.AddListener(value);
            remove => m_OnHoverEnd.RemoveListener(value);
        }
        
        protected bool HoverActive { get; private set; }

        public event UnityAction InteractionEnabledChanged;
        public event UnityAction DescriptionChanged;

        [SerializeField]
        [Tooltip("Is this object interactable, if not, this object will be treated like a normal one.")]
        private bool m_InteractionEnabled = true;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How time it takes to interact with this object. (e.g. for how many seconds should the Player hold the interact button).")]
        private float m_HoldDuration = 0f;

        [SpaceArea]

        [SerializeField]
        [Tooltip("Interactable text (could be used as a name), shows up in the UI when looking at this object.")]
        private string m_InteractTitle;

        [SerializeField, Multiline]
        [Tooltip("Interactable description, shows up in the UI when looking at this object.")]
        private string m_InteractDescription;

        [SpaceArea]

        [SerializeField]
        [Tooltip("Unity event that will be called when a character interacts with this object.")]
        private InteractEvent m_OnInteract;

        [SerializeField]
        private InteractEvent m_OnHoverStart;

        [SerializeField]
        private InteractEvent m_OnHoverEnd;

        [SpaceArea]
        
        [SerializeField, ChildObjectOnly(),  FormerlySerializedAs("m_MaterialChanger")]
        private MaterialEffect m_MaterialEffect;


        /// <summary>
        /// Called when a character interacts with this object.
        /// </summary>
        public virtual void OnInteract(ICharacter character)
        {
            if (m_InteractionEnabled)
                m_OnInteract.Invoke(character);
        }

        /// <summary>
        /// Called when a character starts looking at this object.
        /// </summary>
        public virtual void OnHoverStart(ICharacter character)
        {
            if (!InteractionEnabled)
                return;

            if (m_MaterialEffect != null)
                m_MaterialEffect.EnableDefaultEffect();

            HoverActive = true;
            m_OnHoverStart.Invoke(character);
        }

        /// <summary>
        /// Called when a character stops looking at this object.
        /// </summary>
        public virtual void OnHoverEnd(ICharacter character)
        {
            if (!InteractionEnabled)
                return;

            if (m_MaterialEffect != null)
                m_MaterialEffect.DisableActiveEffect();

            HoverActive = false;
            m_OnHoverEnd.Invoke(character);
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Title = this.GetType().Name.ToUnityLikeNameFormat();
        }

        protected virtual void OnValidate()
        {
            if (m_MaterialEffect == null)
                m_MaterialEffect = transform.root.GetComponentInChildren<MaterialEffect>();
        }
#endif
    }
}