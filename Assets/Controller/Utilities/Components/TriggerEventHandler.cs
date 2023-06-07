using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    [RequireComponent(typeof(Collider))]
    public abstract class TriggerEventHandler<T> : MonoBehaviour
    {
        #region Internal
        [Serializable]
        public class TriggerEvent : UnityEvent<T> { }
        #endregion

        public event UnityAction<T> TriggerEnter
        {
            add => m_TriggerEnter.AddListener(value);
            remove => m_TriggerEnter.RemoveListener(value);
        }

        public event UnityAction<T> TriggerExit
        {
            add => m_TriggerExit.AddListener(value);
            remove => m_TriggerExit.RemoveListener(value);
        }

        [SerializeField]
        protected TriggerEvent m_TriggerEnter;

        [SerializeField]
        protected TriggerEvent m_TriggerExit;


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out T comp))
                m_TriggerEnter.Invoke(comp);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out T comp))
                m_TriggerExit.Invoke(comp);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (TryGetComponent<Collider>(out var col))
                col.isTrigger = true;
        }
#endif
    }
}