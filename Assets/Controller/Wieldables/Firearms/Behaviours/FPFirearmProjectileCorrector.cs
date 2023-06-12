using System.Collections;
using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/First Person/FP Firearm Projectile Corrector")]
    public sealed class FPFirearmProjectileCorrector : MonoBehaviour
    {
        [SerializeField, NotNull]
        private FPFirearmCartridge m_FPCartridge;

        [SerializeField, Range(0f, 10f)]
        private float m_ReloadUpdateDelay = 0.5f;

        private IFirearm m_Firearm;
        private IReloader m_Reloader;
        private WaitForSeconds m_ReloadWait;


        private void Start()
        {
            m_ReloadWait = new WaitForSeconds(m_ReloadUpdateDelay);

            m_Firearm = GetComponentInParent<IFirearm>(true);
            if (m_Firearm != null)
            {
                var wieldable = m_Firearm as IWieldable;
                m_Firearm.ReloaderChanged += OnReloaderChanged;
                OnReloaderChanged(m_Firearm.Reloader);
            }
        }

        private void OnDestroy()
        {
            if (m_Firearm != null)
            {
                var wieldable = m_Firearm as IWieldable;
                m_Firearm.ReloaderChanged -= OnReloaderChanged;
            }
        }

        private void OnReloaderChanged(IReloader reloader)
        {
            // Unsubscribe from previous reloader
            if (m_Reloader != null)
            {
                m_Reloader.AmmoInMagazineChanged -= OnAmmoChanged;
                m_Reloader.ReloadStart -= OnReloadStart;
            }

            // Subscribe to current reloader
            m_Reloader = reloader;

            if (m_Reloader != null)
            {
                m_Reloader.AmmoInMagazineChanged += OnAmmoChanged;
                m_Reloader.ReloadStart += OnReloadStart;
                OnAmmoChanged(m_Reloader.AmmoInMagazine, m_Reloader.AmmoInMagazine);
            }
        }

        private void OnAmmoChanged(int prevInMag, int currentInMag)
        {
            if (m_Reloader.IsReloading)
                return;

            m_FPCartridge.ChangeState(currentInMag != 0);
        }

        private void OnReloadStart() => StartCoroutine(C_ReloadUpdateCartridges(m_Reloader.AmmoToLoad));

        private IEnumerator C_ReloadUpdateCartridges(int reloadingAmount)
        {
            yield return m_ReloadWait;

            if (reloadingAmount > 0)
                m_FPCartridge.ChangeState(true);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                m_ReloadWait = new WaitForSeconds(m_ReloadUpdateDelay);
        }
#endif
    }
}