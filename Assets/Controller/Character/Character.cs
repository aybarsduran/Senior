using IdenticalStudios;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{

    public class Character : MonoBehaviour, ICharacter
    {
        public bool IsInitialized { get; private set; }

        // The property returns a reference to the Transform component named m_View.
        public Transform ViewTransform => m_View;
        public Collider[] Colliders { get; private set; }

        public event UnityAction Initialized;

        //[SerializeField, NotNull]
        private Transform m_View;

        public bool HasCollider(Collider collider)
        {
            for (int i = 0; i < Colliders.Length; i++)
            {
                if (Colliders[i] == collider)
                    return true;
            }

            return false;
        }

        protected virtual void Awake()
        {            
            SetupBaseReferences();
        }

        protected virtual void Start()
        {
            IsInitialized = true;
            Initialized?.Invoke();
        }

        protected virtual void SetupBaseReferences()
        {          
            Colliders = GetComponentsInChildren<Collider>(true);
        }
    }
}
