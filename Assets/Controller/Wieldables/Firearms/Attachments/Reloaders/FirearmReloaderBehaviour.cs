using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmReloaderBehaviour : FirearmAttachmentBehaviour, IReloader
    {
        #region Internal
        protected enum ReloadType
        {
            Standard,
            Progressive
        }
        #endregion

        public bool IsReloading
        {
            get => m_IsReloading;
            protected set
            {
                if (value == m_IsReloading)
                    return;
                
                m_IsReloading = value;

                if (m_IsReloading)
                    ReloadStart?.Invoke();
                else
                    ReloadFinish?.Invoke();
            }
        }

        public int AmmoInMagazine
        {
            get => m_AmmoInMagazine;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, MagazineSize);

                if (clampedValue != m_AmmoInMagazine)
                {
                    int prevInMagazine = m_AmmoInMagazine;
                    m_AmmoInMagazine = clampedValue;
                    AmmoInMagazineChanged?.Invoke(prevInMagazine, m_AmmoInMagazine);
                }
            }
        }

        public virtual int MagazineSize => 0;
        public virtual int AmmoToLoad => 0;

        public bool IsMagazineEmpty => AmmoInMagazine <= 0;
        public bool IsMagazineFull => AmmoInMagazine >= MagazineSize;

        public event UnityAction<int, int> AmmoInMagazineChanged;
        public event UnityAction ReloadFinish;
        public event UnityAction ReloadStart;

        private bool m_IsReloading;
        private int m_AmmoInMagazine;


        protected virtual void OnEnable() => Firearm?.SetReloader(this);

        protected virtual void OnDisable() 
        {
#if UNITY_EDITOR
            if (UnityUtils.IsQuittingPlayMode)
                return;
#endif

            TryCancelReload(Firearm.Ammo, out _);
            IsReloading = false;
        }

        public abstract bool TryStartReload(IAmmo ammoModule);
        public abstract bool TryUseAmmo(int amount);

        public virtual bool TryCancelReload(IAmmo ammoModule, out float endDuration)
        {
            endDuration = 0.5f;

            if (!IsReloading)
                return false;

            ammoModule.AddAmmo(AmmoToLoad);
            IsReloading = false;

            return true;
        }
    }
}