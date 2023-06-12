using IdenticalStudios.InventorySystem;
using IdenticalStudios;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    /// <summary>
    /// Main character class used by every entity in the game.
    /// It acts as a hub for accessing modules.
    /// </summary>
    public class Character : MonoBehaviour, ICharacter
    {
        public bool IsInitialized { get; private set; }
        public Transform ViewTransform => m_View;
        public Collider[] Colliders { get; private set; }

        public IAudioPlayer AudioPlayer { get; private set; }
        public IHealthManager HealthManager { get; private set; }
        public IInventory Inventory { get; private set; }

        /// <summary>
        /// This message will be sent after all modules are created and initialized.
        /// </summary>
        public event UnityAction Initialized;

        [SerializeField]
        [Tooltip("The view transform, you can think of it as the eyes of the character")]
        private Transform m_View;

        private Dictionary<Type, ICharacterModule> m_ModulesByType;
        private static readonly List<ICharacterModule> s_CachedModules = new(32);


        /// <summary>
        /// <para> Returns child module of specified type from this character. </para>
        /// Use this if you are NOT sure this character has a module of the given type.
        /// </summary>
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

        /// <summary>
        /// <para> Returns child module of specified type from this character. </para>
        /// Use this if you ARE sure this character has a module of the given type.
        /// </summary>
        public void GetModule<T>(out T module) where T : class, ICharacterModule
        {
            if (m_ModulesByType != null && m_ModulesByType.TryGetValue(typeof(T), out ICharacterModule charModule))
            {
                module = (T)charModule;
                return;
            }

            module = default;
        }

        /// <summary>
        /// <para> Returns child module of specified type from this character. </para>
        /// Use this if you ARE sure this character has a module of the given type.
        /// </summary>
        public T GetModule<T>() where T : class, ICharacterModule
        {
            if (m_ModulesByType != null && m_ModulesByType.TryGetValue(typeof(T), out ICharacterModule charModule))
                return (T)charModule;

            return default;
        }

        /// <summary>
        /// Returns true if the passed collider is part of this character.
        /// </summary>
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
            SetupModules();
            SetupBaseReferences();
        }

        protected virtual void Start()
        {
            IsInitialized = true;
            Initialized?.Invoke();
        }

        protected virtual void SetupBaseReferences()
        {
            AudioPlayer = GetModule<IAudioPlayer>();
            HealthManager = GetModule<IHealthManager>();
            Inventory = GetModule<IInventory>();

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
