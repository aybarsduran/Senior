using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Triggers/Charging Trigger")]
	public class FirearmChargingTrigger : FirearmTriggerBehaviour, IChargeHandler
	{
		[SerializeField, Range(0f, 10f)]
		[Tooltip("The minimum time that can pass between consecutive shots.")]
		private float m_PressCooldown;

		[SerializeField, Range(0f, 0.95f)]
		[Tooltip("Minimum charge needed to shoot")]
		private float m_MinChargeTime;

		[SerializeField, Range(0f, 10f)]
		private float m_MaxChargeTime = 1f;

		[SerializeField]
		private AnimationCurve m_ChargeCurve;

		[SerializeField]
		private bool m_CanAimUncharged = false;

		[SerializeField, Range(0f, 2f)]
		private float m_FOVSetDuration = 0.4f;

		[SerializeField, Range(0f, 2f)]
		private float m_WorldFOVMod = 0.75f;

		[SerializeField, Range(0f, 2f)]
		private float m_OverlayFOVMod = 0.75f;

		[SerializeField]
		private EffectCollection m_ChargeStartEffects;

		[SerializeField]
		private DynamicEffectCollection m_ChargeEndEffects;

		[SerializeField]
		private EffectCollection m_ChargeMaxEffects;

		private float m_NextTimeCanHold;
		private float m_TriggerChargeStartTime;
		private bool m_TriggerChargeStarted;
		private bool m_ChargeMaxed;

		private FPWieldableFOV m_FOVHandler;
		private IAimInputHandler m_AimInputHandler;


        public override void HoldTrigger()
		{
			if (Time.time < m_NextTimeCanHold)
				return;

			if (Firearm.Reloader.IsReloading || Firearm.Reloader.IsMagazineEmpty)
			{
				RaiseShootEvent(0f);
				return;
			}

			IsTriggerHeld = true;

			if (!m_TriggerChargeStarted && GetNormalizedCharge() > m_MinChargeTime)
				OnChargeStart();

			if (!m_ChargeMaxed && GetNormalizedCharge() > (m_MaxChargeTime - 0.01f))
				OnChargeMaxed();
		}

		public override void ReleaseTrigger()
		{
			if (!IsTriggerHeld)
				return;

			OnChargeEnd();
		}

        public float GetNormalizedCharge()
        {
			if (!IsTriggerHeld)
				return 0f;

			float normalizedCharge = (Time.time - m_TriggerChargeStartTime) / m_MaxChargeTime;
			normalizedCharge = Mathf.Clamp(normalizedCharge, 0.05f, 1f);

			return normalizedCharge;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			m_AimInputHandler = Wieldable as IAimInputHandler;
			ResetCharge();
		}

        protected override void OnDisable()
        {
            base.OnDisable();
			UnblockAimer();
		}

		protected override void Awake()
		{
			base.Awake();
			m_FOVHandler = Wieldable.gameObject.GetComponentInFirstChildren<FPWieldableFOV>();
		}

		private void ResetCharge()
		{
			m_TriggerChargeStarted = false;
			m_ChargeMaxed = false;

			m_NextTimeCanHold = Time.time + m_PressCooldown;
			IsTriggerHeld = false;

			BlockAimer();
		}

		private void OnChargeStart()
		{
			m_FOVHandler.SetOverlayFOV(m_OverlayFOVMod, m_FOVSetDuration * 1.1f);
			m_FOVHandler.SetWorldFOV(m_WorldFOVMod, m_FOVSetDuration);
			m_FOVHandler.SetDOFMod(1f);

			m_ChargeStartEffects.PlayEffects(Wieldable);

			m_TriggerChargeStarted = true;
			m_TriggerChargeStartTime = Time.time;

			UnblockAimer();
		}

		private void OnChargeEnd()
		{
			m_FOVHandler.SetOverlayFOV(1f, m_FOVSetDuration * 0.9f);
			m_FOVHandler.SetWorldFOV(1f, m_FOVSetDuration * 0.9f);
			m_FOVHandler.SetDOFMod(0f);

			if (GetNormalizedCharge() >= m_MinChargeTime)
			{
				float normalizedCharge = GetNormalizedCharge();
				float chargeAmount = normalizedCharge * m_ChargeCurve.Evaluate(normalizedCharge);

				m_ChargeEndEffects.PlayEffects(Wieldable, chargeAmount);

				RaiseShootEvent(Mathf.Clamp(chargeAmount, 0.3f, 1f));
			}

			ResetCharge();
		}

		private void OnChargeMaxed()
		{
			m_ChargeMaxEffects.PlayEffects(Wieldable);
			m_ChargeMaxed = true;
		}

		private void BlockAimer()
		{
			if (!m_CanAimUncharged)
				m_AimInputHandler.AimBlocker.AddBlocker(this);
		}

		private void UnblockAimer() 
		{
			if (!m_CanAimUncharged)
				m_AimInputHandler.AimBlocker.RemoveBlocker(this);
		}
	}
}