using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Reloaders/Basic Reloader")]
    public class FirearmBasicReloader : FirearmReloaderBehaviour
	{
        public override int MagazineSize => m_MagazineSize;
        public override int AmmoToLoad => ammoToLoad;

        [SerializeField, Range(0,500)]
        private int m_MagazineSize;
        
        [SerializeField, Range(0.01f, 15f)]
        private float m_ReloadDuration;

        [SerializeField]
        private EffectCollection m_ReloadEffects;

        protected float reloadEndTime;
        protected int ammoToLoad;


        public override bool TryUseAmmo(int amount)
        {
            if (IsMagazineEmpty || AmmoInMagazine < amount)
                return false;

            AmmoInMagazine -= amount;

            return true;
        }

        public override bool TryCancelReload(IAmmo ammoModule, out float endDuration)
        {
            endDuration = 0f;
            return false;
        }

        public override bool TryStartReload(IAmmo ammoModule)
		{
            if (IsReloading || IsMagazineFull)
                return false;

            ammoToLoad = ammoModule.RemoveAmmo(MagazineSize - AmmoInMagazine);

            if (ammoToLoad > 0)
            {
                reloadEndTime = Time.time + m_ReloadDuration;

                m_ReloadEffects.PlayEffects(Wieldable);

                IsReloading = true;

                return true;
            }

            return false;
		}

        protected virtual void Update()
        {
            if (!IsReloading)
                return;
            
            if (Time.time > reloadEndTime)
            {
                AmmoInMagazine += ammoToLoad;

                IsReloading = false;
            }
        }
    }
}