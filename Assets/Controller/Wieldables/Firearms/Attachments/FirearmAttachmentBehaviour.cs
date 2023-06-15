using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmAttachmentBehaviour : MonoBehaviour
    {
        #region Internal
        private enum EnableMode
        {
            Component,
            Gameobject
        }
        #endregion

        protected IWieldable Wieldable { get; private set; }
        protected IFirearm Firearm { get; private set; }

        public bool AttachOnStart
        {
            get => m_AttachOnStart;
            set => m_AttachOnStart = value;
        }

        [SerializeField]
        private EnableMode m_EnableMode;

        [SerializeField, HideInInspector]
        private bool m_AttachOnStart;
        
        
        public void Attach()
        {
            switch (m_EnableMode)
            {
                case EnableMode.Gameobject:
                    gameObject.SetActive(true);
                    break;
                case EnableMode.Component:
                    enabled = true;
                    break;
            }
        }

        public void Detach()
        {
            switch (m_EnableMode)
            {
                case EnableMode.Gameobject:
                    gameObject.SetActive(false);
                    break;
                case EnableMode.Component:
                    enabled = false;
                    break;
            }
        }

        protected virtual void Awake()
        {
            Wieldable = GetComponentInParent<IWieldable>();
            Firearm = GetComponentInParent<IFirearm>();
            
            if (m_AttachOnStart)
                Attach();
            else
                Detach();
        }

#if UNITY_EDITOR
        public static event UnityAction EffectCollectionsRefreshed;
        public void RefreshEffectCollections() => EffectCollectionsRefreshed?.Invoke();
#endif
    }
}