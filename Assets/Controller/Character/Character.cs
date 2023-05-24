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

        public IAudioPlayer AudioPlayer { get; private set; }
        public IHealthManager HealthManager { get; private set; }

        public event UnityAction Initialized;

        //[SerializeField, NotNull]
        private Transform m_View;

        private Dictionary<Type, ICharacterModule> m_ModulesByType;
        private static readonly List<ICharacterModule> s_CachedModules = new(32);


        
        // Returns child module of specified type from this character.
        public bool TryGetModule<T>(out T module) where T : class, ICharacterModule
        {
            if (m_ModulesByType != null && m_ModulesByType.TryGetValue(typeof(T), out ICharacterModule charModule))
            {
                module = (T)charModule;
                return true;
            }
            else
            {
                module = default;
                return false;
            }
        }

     
        // Returns child module of specified type from this character.
        public void GetModule<T>(out T module) where T : class, ICharacterModule
        {
            if (m_ModulesByType != null && m_ModulesByType.TryGetValue(typeof(T), out ICharacterModule charModule))
            {
                module = (T)charModule;
                return;
            }

            module = default;
        }

      
        // Returns child module of specified type from this character. 
        public T GetModule<T>() where T : class, ICharacterModule
        {
            if (m_ModulesByType != null && m_ModulesByType.TryGetValue(typeof(T), out ICharacterModule charModule))
                return (T)charModule;

            return default;
        }

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

        protected virtual void Start() // virtual alt siniflarýn methodu yeniden tanýimlayabilmesi icin
        {
            IsInitialized = true;
            Initialized?.Invoke();
        }

        protected virtual void SetupBaseReferences()
        {          
            Colliders = GetComponentsInChildren<Collider>(true);
        }

        private void SetupModules()
        {
            // Find & Setup all of the Modules
            GetComponentsInChildren(s_CachedModules);
            for (int i = 0; i < s_CachedModules.Count; i++)
            {
                ICharacterModule module = s_CachedModules[i];

                Type[] interfaces = module.GetType().GetInterfaces();
                foreach (Type interfaceType in interfaces)
                {
                    if (interfaceType.GetInterface(typeof(ICharacterModule).Name) != null)
                    {
                        if (m_ModulesByType == null)
                            m_ModulesByType = new Dictionary<Type, ICharacterModule>();

                        if (!m_ModulesByType.ContainsKey(interfaceType))
                            m_ModulesByType.Add(interfaceType, module);
                        //else
                        //    Debug.LogError($"2 Modules of the same type ({module.GetType()}) found under {gameObject.name}.");
                    }
                }
            }
        }
    }
}
