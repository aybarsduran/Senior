using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Reloaders/Standard Reloader")]
    public class FirearmStandardReloader : FirearmBasicReloader
    {
        [SerializeField, Range(0.01f, 15f)]
        private float m_EmptyReloadDuration = 3f;

        [SerializeField]
        private WieldableVFXBehaviour m_EmptyReloadEffect;

        [SerializeField]
        private EffectCollection m_EmptyReloadEffects;
        
        
        public override bool TryStartReload(IAmmo ammoModule)
        {
            if (IsReloading || IsMagazineFull)
                return false;

            if (!IsMagazineEmpty)
                return base.TryStartReload(ammoModule);

            ammoToLoad = ammoModule.RemoveAmmo(MagazineSize - AmmoInMagazine);

            // Empty Reload
            reloadEndTime = Time.time + m_EmptyReloadDuration;

            // Empty Reload Visual Effect
            if (m_EmptyReloadEffect != null)
                m_EmptyReloadEffect.PlayEffect();

            // Effects
            m_EmptyReloadEffects.PlayEffects(Wieldable);
            
            IsReloading = true;

            return true;
        }
    }
}