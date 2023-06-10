using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
	[AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Triggers/Full Auto Trigger")]
	public class FirearmFullAutoTrigger : FirearmTriggerBehaviour
	{
		[SerializeField, Range(0, 10000)]
		[Tooltip("The maximum amount of shots that can be executed in a minute.")]
		private int m_RoundsPerMinute = 450;

		private float m_NextTimeCanShoot;


		public override void HoldTrigger()
		{
			base.HoldTrigger();

			if (Time.time < m_NextTimeCanShoot)
				return;

			RaiseShootEvent(1f);

			m_NextTimeCanShoot = Time.time + (60f / m_RoundsPerMinute);
		}

		protected override void TapTrigger()
		{
			if (Time.time < m_NextTimeCanShoot)
				return;

			if (Firearm.Reloader.IsReloading || Firearm.Reloader.IsMagazineEmpty)
				RaiseShootEvent(1f);
		}
	}
}