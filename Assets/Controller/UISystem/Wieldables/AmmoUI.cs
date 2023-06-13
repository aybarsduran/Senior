using IdenticalStudios.ProceduralMotion;
using IdenticalStudios.WieldableSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class AmmoUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current ammo in the magazine.")]
        private TextMeshProUGUI m_MagazineText;

        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current ammo in the storage.")]
        private TextMeshProUGUI m_StorageText;

        [SerializeField]
        private Image m_InfiniteStorageImage;

        [SerializeField]
        private Gradient m_MagazineColor;

        [Title("Animation")]

        [SerializeField]
        private TweenSequence m_ShowAnimation;

        [SerializeField]
        private TweenSequence m_UpdateAnimation;

        [SerializeField]
        private TweenSequence m_HideAnimation;

        private IWieldablesController m_WieldableController;
        private IReloader m_Reloader;
        private IAmmo m_Ammo;
        private IFirearm m_Firearm;


        protected override void OnAttachment()
        {
            m_InfiniteStorageImage.enabled = false;

            GetModule(out m_WieldableController);
            m_WieldableController.WieldableEquipStopped += OnWieldableEquipped;

            m_HideAnimation.PlayAnimation();
        }

        private void OnWieldableEquipped(IWieldable wieldable)
        {
            // Unsubscribe from previous firearm
            if (m_Firearm != null)
            {
                m_Firearm.ReloaderChanged -= OnReloaderChanged;
                m_Firearm.AmmoChanged -= OnAmmoChanged;

                m_Firearm = null;
            }

            if (wieldable is IFirearm firearm)
            {
                if (m_Firearm == null)
                {
                    m_HideAnimation.CancelAnimation();
                    m_ShowAnimation.PlayAnimation();
                }

                // Subscribe to current firearm
                m_Firearm = firearm;
                m_Firearm.ReloaderChanged += OnReloaderChanged;
                m_Firearm.AmmoChanged += OnAmmoChanged;

                OnReloaderChanged(m_Firearm.Reloader);
                OnAmmoChanged(m_Firearm.Ammo);
            }
            else
                m_HideAnimation.PlayAnimation();
        }

        private void OnReloaderChanged(IReloader currentReloader)
        {
            // Prev reloader
            if (m_Reloader != null)
                m_Reloader.AmmoInMagazineChanged -= UpdateMagazineText;

            // Current reloader
            if (currentReloader != null)
            {
                m_Reloader = currentReloader;

                m_Reloader.AmmoInMagazineChanged += UpdateMagazineText;
                UpdateMagazineText(m_Reloader.AmmoInMagazine, m_Reloader.AmmoInMagazine);
            }
        }

        private void OnAmmoChanged(IAmmo currentAmmo)
        {
            // Prev ammo
            if (m_Ammo != null)
                m_Ammo.AmmoCountChanged -= UpdateStorageText;

            // Current ammo
            if (currentAmmo != null)
            {
                m_Ammo = currentAmmo;

                m_Ammo.AmmoCountChanged += UpdateStorageText;
                UpdateStorageText(m_Ammo.GetAmmoCount());
            }
        }

        private void UpdateMagazineText(int prevAmmo, int currentAmmo) 
        {
            m_MagazineText.text = currentAmmo.ToString();
            m_MagazineText.color = m_MagazineColor.Evaluate((float)currentAmmo / (float)m_Reloader.MagazineSize);

            if (prevAmmo > currentAmmo)
                m_UpdateAnimation.PlayAnimation();
        }

        private void UpdateStorageText(int currentAmmo)
        {
            if (currentAmmo > 100000)
            {
                m_StorageText.text = string.Empty;
                m_InfiniteStorageImage.enabled = true;
            }
            else
            {
                m_StorageText.text = currentAmmo.ToString();
                m_InfiniteStorageImage.enabled = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_ShowAnimation?.OnValidate(gameObject);
            m_UpdateAnimation?.OnValidate(gameObject);
            m_HideAnimation?.OnValidate(gameObject);
        }
#endif
    }
}