using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
	[AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Triggers/Hold Trigger")]
	public class FirearmHoldTrigger : FirearmTriggerBehaviour
	{
		[SerializeField, Range(0f, 10f)]
		[Tooltip("The minimum time that can pass between consecutive shots.")]
		private float m_PressCooldown = 0.22f;

		[SerializeField, Range(0f, 3f)]
		private float m_PressTime = 0f;

		[SerializeField]
		private DynamicEffectCollection m_TriggerHoldStartEffects;

		private float m_NextTimeCanPress;
		private float m_TriggerHoldTime;
		private bool m_CanHoldTrigger;


		public override void HoldTrigger()
		{
			base.HoldTrigger();

			if (!m_CanHoldTrigger)
				return;
			
			m_TriggerHoldTime += Time.deltaTime;

			if (m_TriggerHoldTime > m_PressTime && Time.time > m_NextTimeCanPress)
			{
				RaiseShootEvent(1f);

				m_NextTimeCanPress = Time.time + m_PressCooldown;
				m_CanHoldTrigger = false;
			}

			m_TriggerHoldStartEffects.PlayEffects(Wieldable, m_TriggerHoldTime);
		}

		protected override void TapTrigger()
		{
			m_CanHoldTrigger = true;
			m_TriggerHoldTime = 0f;

			if (Time.time > m_NextTimeCanPress)
			{
				RaiseShootEvent(1f);

				m_NextTimeCanPress = Time.time + m_PressCooldown;
			}
		}
	}
}