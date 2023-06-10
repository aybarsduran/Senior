using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    /// <summary>
    /// Controller responsible for equipping and holstering wieldables.
    /// </summary>
    [RequireComponent(typeof(CharacterAccuracyHandler))]
    public sealed class WieldablesController : CharacterBehaviour, IWieldablesController
    {
        public IWieldable ActiveWieldable { get; private set; }
        public bool IsEquipping { get; private set; }

        public event WieldableEquipDelegate WieldableEquipStopped;
        public event WieldableEquipDelegate WieldableEquipStarted;
        public event WieldableEquipDelegate WieldableHolsterStarted;

        [SerializeField]
        [Tooltip("The parent of the spawned wieldables.")]
        private Transform m_SpawnRoot;
        
        [SerializeField]
        [Tooltip("The audio player that every wieldable will use.")]
        private AudioPlayer m_AudioPlayer;
        
        [SerializeField]
        private CharacterAccuracyHandler m_AccuracyHandler;


        [SerializeField]//The wieldable that will be equipped when equipping a NULL wieldable, can be left empty. (Tip: you can set it to an arms/unarmed wieldable).")]
        private Wieldable m_DefaultWieldable;

        private readonly List<IWieldable> m_Wieldables = new();
        private UnityAction m_EquippedCallback;


        protected override void OnBehaviourEnabled()
        {
            if (m_DefaultWieldable != null)
            {
                m_DefaultWieldable = AddWieldable(m_DefaultWieldable) as Wieldable;
                TryEquipWieldable(m_DefaultWieldable);
            }
        }

        public bool GetWieldableOfType<T>(out T wieldable) where T : IWieldable
        {
            if (m_Wieldables != null)
            {
                foreach (var w in m_Wieldables)
                {
                    if (w.GetType() == typeof(T))
                    {
                        wieldable = (T)w;
                        return true;
                    }
                }
            }

            wieldable = default;

            return false;
        }

        public IWieldable AddWieldable(IWieldable wieldable, bool spawn = true, bool disable = true)
        {
            IWieldable newWieldable;

            if (spawn)
                newWieldable = Instantiate(wieldable.gameObject, transform.position, transform.rotation, m_SpawnRoot != null ? m_SpawnRoot : transform).GetComponent<IWieldable>();
            else
                newWieldable = wieldable;

            m_Wieldables.Add(newWieldable);

            newWieldable.Character = Character;
            newWieldable.AudioPlayer = m_AudioPlayer;
            newWieldable.AccuracyHandler = m_AccuracyHandler;

            if (disable)
            {
                newWieldable.SetVisibility(true);
                StartCoroutine(C_HideDelayed(newWieldable));
            }

            return newWieldable;
        }

        public bool RemoveWieldable(IWieldable wieldable, bool destroy = false)
        {
            if (!HasWieldable(wieldable)) 
                return false;
            
            if (ActiveWieldable == wieldable)
            {
                ActiveWieldable.OnHolster(10f);
                ActiveWieldable.SetVisibility(false);
            }

            if (destroy)
                Destroy(wieldable.gameObject, 1f);

            return true;
        }

        public bool HasWieldable(IWieldable wieldable) => m_Wieldables.Contains(wieldable);

        public bool TryEquipWieldable(IWieldable wieldable, float holsterPreviousSpeedMod = 1f, UnityAction equippedCallback = null)
        {
            bool canEquip = !IsEquipping && (wieldable == null || HasWieldable(wieldable));

            if (canEquip)
            {
                m_EquippedCallback = equippedCallback;
                holsterPreviousSpeedMod = Mathf.Clamp(holsterPreviousSpeedMod, 0.25f, 5f);
                StartCoroutine(C_EquipWieldable(wieldable, holsterPreviousSpeedMod));
            }

            return canEquip;
        }

        // Holsters & Hides the active wieldable then enables and equips the given one.
        private IEnumerator C_EquipWieldable(IWieldable wieldableToEquip, float holsterPrevSpeedMod = 1f)
        {
            IsEquipping = true;
            WieldableHolsterStarted?.Invoke(ActiveWieldable);

            // Holster and disables the previous wieldable.
            if (ActiveWieldable != null)
            {
                float hideDelay = ActiveWieldable.HolsterDuration / holsterPrevSpeedMod;
                ActiveWieldable.OnHolster(holsterPrevSpeedMod);

                yield return new WaitForSeconds(hideDelay);

                // Hides the previous wieldable.
                ActiveWieldable.SetVisibility(false);
            }

            if (wieldableToEquip == null)
                wieldableToEquip = m_DefaultWieldable;

            ActiveWieldable = wieldableToEquip;

            // Enables and equips the new wieldable.
            if (wieldableToEquip != null)
            {
                wieldableToEquip.SetVisibility(true);
                wieldableToEquip.OnEquip();
            }

            WieldableEquipStarted?.Invoke(wieldableToEquip);
            m_EquippedCallback?.Invoke();

            if (wieldableToEquip != null)
                yield return new WaitForSeconds(wieldableToEquip.EquipDuration);

            IsEquipping = false;
            WieldableEquipStopped?.Invoke(wieldableToEquip);
        }

        private IEnumerator C_HideDelayed(IWieldable wieldable)
        {
            yield return null;
            yield return null;
            wieldable.SetVisibility(false);
        }
    }
}