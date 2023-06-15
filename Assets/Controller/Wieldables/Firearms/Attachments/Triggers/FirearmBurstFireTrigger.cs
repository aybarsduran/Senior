using System.Collections;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
	[AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Triggers/Burst Fire Trigger")]
	public class FirearmBurstFireTrigger : FirearmTriggerBehaviour
    {
		[SpaceArea]
		[SerializeField, Range(0, 100)]
		[Tooltip("How many times in succesion will the trigger be pressed..")]
		private int m_BurstLength = 3;

		[SerializeField, Range(0f, 100f)]
		[Tooltip("How much time it takes to complete the burst.")]
		private float m_BurstDuration = 0.3f;

		[SerializeField, Range(0f, 100f)]
		[Tooltip("The minimum time that can pass between consecutive bursts.")]
		private float m_BurstPause = 0.35f;

		private float m_NextTimeCanShoot;
		private float m_ShootThreshold;
		private WaitForSeconds m_BurstWait;


		protected override void TapTrigger()
		{
			if (Time.time < m_NextTimeCanShoot)
				return;

			if (Firearm.Reloader.IsReloading || Firearm.Reloader.IsMagazineEmpty)
				RaiseShootEvent(1f);
			else
				StartCoroutine(C_DoBurst());

			m_NextTimeCanShoot = Time.time + m_ShootThreshold;
		}

		private IEnumerator C_DoBurst()
		{
			for (int i = 0; i < m_BurstLength; i++)
			{
				RaiseShootEvent(1f);
				yield return m_BurstWait;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			m_BurstWait = new WaitForSeconds(m_BurstDuration / m_BurstLength);
			m_ShootThreshold = m_BurstDuration + m_BurstPause;
		}

#if UNITY_EDITOR
		private void OnValidate() 
		{
			m_BurstWait = new WaitForSeconds(m_BurstDuration / m_BurstLength);
			m_ShootThreshold = m_BurstDuration + m_BurstPause;
		}
#endif
	}
}