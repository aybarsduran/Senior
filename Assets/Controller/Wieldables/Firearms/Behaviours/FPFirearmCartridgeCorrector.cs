using System.Collections;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class FPFirearmCartridgeCorrector : MonoBehaviour
    {
        [SerializeField]
        private FPFirearmCartridge m_CartridgePrefab = null;

        [SerializeField, Range(0f, 10f)]
        private float m_ReloadUpdateDelay = 0.5f;


        [SerializeField]
        private Transform[] m_SpawnTransforms = null;

        private IReloader m_Reloader;
        private IFirearm m_Firearm;

        private FPFirearmCartridge[] m_FPCartridges = null;
        private WaitForSeconds m_ReloadWait;


        private void Awake()
        {
            InitializeVisualCartridges();

            m_Firearm = GetComponentInParent<IFirearm>();
            m_ReloadWait = new WaitForSeconds(m_ReloadUpdateDelay);
        }

        private void Start()
        {
            m_Firearm.ReloaderChanged += OnReloaderChanged;
            OnReloaderChanged(m_Firearm.Reloader);
        }

        private void InitializeVisualCartridges() 
        {
            if (m_CartridgePrefab != null)
            {
                m_FPCartridges = new FPFirearmCartridge[m_SpawnTransforms.Length];

                for (int i = 0; i < m_FPCartridges.Length; i++)
                    m_FPCartridges[i] = Instantiate(m_CartridgePrefab, m_SpawnTransforms[i]);
            }
        }

        private void OnDestroy()
        {
            if (m_Firearm != null)
                m_Firearm.ReloaderChanged -= OnReloaderChanged;
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
            m_Reloader.AmmoInMagazineChanged += OnAmmoChanged;
            m_Reloader.ReloadStart += OnReloadStart;

            OnAmmoChanged(m_Reloader.AmmoInMagazine, m_Reloader.AmmoInMagazine);
        }

        private void OnAmmoChanged(int prevInMag, int currentInMag)
        {
            if (m_FPCartridges == null || m_Reloader.IsReloading)
                return;

            int magSize = m_Reloader.MagazineSize;

            if (magSize - (magSize - currentInMag) < m_FPCartridges.Length)
                m_FPCartridges[magSize - (magSize - currentInMag)].ChangeState(false);
        }

        private void OnReloadStart() => StartCoroutine(C_ReloadUpdateCartridges(m_Reloader.AmmoInMagazine, m_Reloader.AmmoToLoad));

        private IEnumerator C_ReloadUpdateCartridges(int currentInMag, int reloadingAmount)
        {
            yield return m_ReloadWait;

            int numberOfCartridgesToEnable = Mathf.Clamp(reloadingAmount - currentInMag + 1, 0, m_FPCartridges.Length);

            for (int i = 0; i < numberOfCartridgesToEnable; i++)
                m_FPCartridges[i].ChangeState(true);
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