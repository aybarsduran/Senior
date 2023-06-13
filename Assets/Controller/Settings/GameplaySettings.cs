using UnityEngine;

namespace IdenticalStudios
{
    [CreateAssetMenu(menuName = "Identical Studios/Settings/Gameplay Settings", fileName = "GameplaySettings")]
    public sealed class GameplaySettings : UserSettings<GameplaySettings>
    {
        public Color CrosshairColor => m_CrosshairColor;
        public bool CanAimWhileShooting => m_CanAimWhileShooting;
        public bool CanAimWhileReloading => m_CanAimWhileReloading;
        public bool CancelReloadOnShoot => m_CancelReloadOnShoot;
        public bool AutoReloadOnDry => m_AutoReloadOnDry;
        
        [Title("HUD")]
        
        [SerializeField]
        private Color m_CrosshairColor = Color.white;

        [Title("Aim")]

        [SerializeField]
        private bool m_CanAimWhileShooting = true;

        [SerializeField]
        private bool m_CanAimWhileReloading = false;

        [Title("Reload")]

        [SerializeField]
        private bool m_CancelReloadOnShoot = true;

        [SerializeField]
        private bool m_AutoReloadOnDry = true;
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
        private static void Init() => SetInstance();
    }
}