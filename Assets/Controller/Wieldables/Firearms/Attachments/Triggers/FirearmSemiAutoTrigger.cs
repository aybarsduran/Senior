using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
	public class FirearmSemiAutoTrigger : FirearmTriggerBehaviour
    {
		[SerializeField, Range(0f, 10f)]
		[Tooltip("The minimum time that can pass between consecutive shots.")]
		private float m_PressCooldown = 0f;

		private float m_NextTimeCanShoot;


        protected override void TapTrigger()
        {
			if (Time.time < m_NextTimeCanShoot)
				return;

			RaiseShootEvent(1f);
			m_NextTimeCanShoot = Time.time + m_PressCooldown;
		}
	}
}