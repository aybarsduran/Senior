using IdenticalStudios.Surfaces;
using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Melee/Basic Swing")]
	public class BasicMeleeSwing : MeleeSwingBehaviour
	{
		public override float SwingDuration => m_AttackThreeshold;
		public override float AttackEffort => m_AttackEffort;
		protected RaycastHit HitInfo => m_HitInfo;

		[Title("Attack")]

		[SerializeField]
		protected DamageType m_DamageType = DamageType.Hit;

		[SerializeField, Range(0f, 3f)]
		protected float m_AttackThreeshold = 0.3f;

		[SerializeField, Range(0f, 5f)]
		protected float m_AttackDelay = 0.2f;

		[SerializeField, Range(0f, 1f)]
		protected float m_AttackEffort = 0.05f;

		[Title("Object Detection")]

		[SerializeField]
		protected LayerMask m_HitMask = (LayerMask)172545;

		[SerializeField]
		protected QueryTriggerInteraction m_HitTriggers;

		[SerializeField]
		protected Vector3 m_HitOffset = Vector3.zero;
		
		[SerializeField, Range(0f, 1f)]
		protected float m_HitRadius = 0.1f;

		[SerializeField, Range(0f, 5f)]
		protected float m_HitDistance = 0.5f;

		[Title("Impact")]

		[SerializeField, Range(0f, 1000f)]
		protected float m_Damage = 15f;

		[SerializeField, Range(0f, 1000f)]
		protected float m_Force = 30f;

		[SerializeField, Range(0f, 100f)]
		protected float m_DurabilityRemove = 5f;

		[Title("Effects")]

		[SerializeField]
		protected EffectCollection m_SwingEffects;

		[SerializeField]
		protected EffectCollection m_HitEffects;

		protected RaycastHit m_HitInfo;

		
		public override bool GetSwingValidity(float accuracy) => true;

		public override void DoSwing(float accuracy)
		{
			UpdateManager.InvokeDelayedAction(this, DoHit, m_AttackDelay);

			// Play Effects.
			m_SwingEffects.PlayEffects(Wieldable);

			// Remove Stamina.
			if (m_AttackEffort > 0.001f)
			{
				if (Wieldable.Character.TryGetModule(out IStaminaController stamina))
					stamina.Stamina -= m_AttackEffort;
			}

			void DoHit() => TryHit(GetUseRay(accuracy));
		}

		public override void CancelSwing() => UpdateManager.StopAllDelayedActionsFor(this);

		protected virtual void OnHit(Ray ray)
		{
			bool isDynamicObject = false;

			// Apply an impact impulse
			if (m_HitInfo.rigidbody != null)
			{
				m_HitInfo.rigidbody.AddForceAtPosition(ray.direction * m_Force, m_HitInfo.point, ForceMode.Impulse);
				isDynamicObject = true;
			}

			if (m_HitInfo.collider.TryGetComponent(out IDamageReceiver receiver))
				receiver.HandleDamage(m_Damage, new DamageContext(m_DamageType, m_HitInfo.point, ray.direction * m_Force, m_HitInfo.normal, Wieldable.Character));

			// Surface effect
			SurfaceManager.SpawnEffect(m_HitInfo, SurfaceEffects.Slash, 1f, isDynamicObject);

			m_HitEffects.PlayEffects(Wieldable);

			ConsumeDurability(m_DurabilityRemove);
		}

		protected Ray GetUseRay(float accuracy)
		{
			float spread = Mathf.Lerp(1f, 5f, 1 - accuracy);
			return PhysicsUtils.GenerateRay(Wieldable.Character.ViewTransform, spread, m_HitOffset);
		}

		private void TryHit(Ray ray)
		{
			if (PhysicsUtils.SphereCastNonAlloc(ray, m_HitRadius, m_HitDistance, out m_HitInfo, m_HitMask, Wieldable.Character.Colliders, m_HitTriggers))
				OnHit(ray);
		}
    }
}