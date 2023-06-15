using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Reloaders/Progressive Reloader")]
	public class FirearmProgressiveReloader : FirearmReloaderBehaviour
	{
		public override int MagazineSize => m_MagazineSize;
		public override int AmmoToLoad => m_AmmoToLoad;

		[SpaceArea]

		[SerializeField, Range(0, 500)]
		private int m_MagazineSize;

		[Title("Tactical Reload")]

		[SerializeField, Range(0f, 10f)]
		private float m_ReloadStartDuration = 0.5f;

		[SerializeField, Range(0f, 10f)]
		private float m_ReloadLoopDuration = 0.35f;

		[SerializeField, Range(0f, 10f)]
		private float m_ReloadEndDuration = 0.5f;

		[Title("Empty Reload")]

		[SerializeField]
		private ReloadType m_EmptyReloadType = ReloadType.Standard;

		[SerializeField, Range(0f, 15f)]
		private float m_EmptyReloadDuration = 3f;

		[SerializeField]
		private WieldableObjectVFX m_EmptyReloadVFX;

		[Title("Effects")]
		
		[SerializeField]
		private EffectCollection m_ReloadStartEffects;

		[SerializeField]
		private EffectCollection m_ReloadLoopEffects;

		[SerializeField]
		private EffectCollection m_ReloadEndEffects;

		[SerializeField]
		private EffectCollection m_EmptyReloadEffects;
		
		private IAmmo m_AmmoModule;
		private int m_AmmoToLoad;

		private bool m_ReloadLoopActive;
		private float m_ReloadLoopEndTime;
		private float m_ReloadLoopStartTime;


		public override bool TryUseAmmo(int amount)
		{
			if (IsMagazineEmpty || AmmoInMagazine < amount)
				return false;

			AmmoInMagazine -= amount;

			return true;
		}

		public override bool TryCancelReload(IAmmo ammoModule, out float endDuration)
		{
			endDuration = m_ReloadEndDuration;

			if (!IsReloading)
				return false;

			m_AmmoToLoad = 0;
			EndReload();

			return true;
		}

		public override bool TryStartReload(IAmmo ammoModule)
		{
			if (IsReloading || IsMagazineFull)
				return false;

			m_AmmoToLoad = MagazineSize - AmmoInMagazine;
			int currentInStorage = ammoModule.GetAmmoCount();

			if (currentInStorage < m_AmmoToLoad)
				m_AmmoToLoad = currentInStorage;

			if (!IsMagazineFull && m_AmmoToLoad > 0)
			{
				// Start Empty Reload
				if (IsMagazineEmpty)
				{
					m_ReloadLoopStartTime = Time.time + m_EmptyReloadDuration;

					// Visual Effect
					if (m_EmptyReloadVFX != null)
						m_EmptyReloadVFX.PlayEffect();

					// Effects
					m_EmptyReloadEffects.PlayEffects(Wieldable);
				}
				// Start Tactical Reload
				else
				{
					m_ReloadLoopStartTime = Time.time + m_ReloadStartDuration;

					// Effects
					m_ReloadStartEffects.PlayEffects(Wieldable);
				}

				IsReloading = true;
				m_ReloadLoopActive = false;

				m_AmmoModule = ammoModule;

				return true;
			}

			return false;
		}

		private void Update()
		{
			if (!IsReloading)
				return;

			// Start reload loop
			if (!m_ReloadLoopActive && Time.time > m_ReloadLoopStartTime)
				StartReloadLoop();

			// Update Reload loop
			if (m_ReloadLoopActive && Time.time > m_ReloadLoopEndTime)
				UpdateReloadLoop();
		}

		private void StartReloadLoop() 
		{
			// Empty Reload
			if (IsMagazineEmpty)
			{
				if (m_EmptyReloadType == ReloadType.Progressive)
				{
					m_AmmoModule.RemoveAmmo(1);
					AmmoInMagazine++;
					m_AmmoToLoad--;

					m_ReloadLoopEndTime = Time.time + m_ReloadStartDuration;

					// Effects
					m_ReloadStartEffects.PlayEffects(Wieldable);
				}
				else
				{
					m_AmmoModule.RemoveAmmo(m_AmmoToLoad);
					AmmoInMagazine += m_AmmoToLoad;
					m_AmmoToLoad = 0;

					IsReloading = false;

					// No need to start the loop since the firearm is fully reloaded
					return;
				}
			}

			m_ReloadLoopActive = true;
		}

		private void UpdateReloadLoop() 
		{
			if (m_AmmoToLoad > 0)
			{
				m_AmmoModule.RemoveAmmo(1);
				AmmoInMagazine++;
				m_AmmoToLoad--;

				m_ReloadLoopEndTime = Time.time + m_ReloadLoopDuration;

				// Effects
				m_ReloadLoopEffects.PlayEffects(Wieldable);
			}
			else
				EndReload();
		}

		private void EndReload()
		{
			if (!IsReloading)
				return;

			IsReloading = false;
			m_ReloadEndEffects.PlayEffects(Wieldable);
		}
	}
}