using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
    /// Will register damage events from outside and pass them to the parent character.
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
	public class CharacterHitbox : CharacterBehaviour, IDamageReceiver
	{
		public Collider Collider => m_Collider;
		public Rigidbody Rigidbody => m_Rigidbody;

		[SerializeField]
		protected bool m_IsCritical;

		[SerializeField, Range(0f, 100f)]
		protected float m_DamageMultiplier = 1f;

		protected Collider m_Collider;
		protected Rigidbody m_Rigidbody;
		protected IHealthManager m_Health;


		public virtual DamageResult HandleDamage(float damage, DamageContext context)
		{
			if (enabled && m_Health != null)
			{
                if (context.Source != (ICharacter)Character && m_Health.IsAlive)
                {
					damage *= m_DamageMultiplier;

					m_Health.ReceiveDamage(damage, context);

					if (!m_Health.IsAlive)
					{
						if (m_Rigidbody != null)
							m_Rigidbody.AddForceAtPosition(context.HitForce, context.HitPoint, ForceMode.Impulse);
					}

					return m_IsCritical ? DamageResult.Critical : DamageResult.Default;
				}
			}
			
			return DamageResult.Ignored;
		}

		protected override void OnBehaviourEnabled()
        {
			m_Health = Character.HealthManager;

			if (m_Health == null)
			{
				Debug.LogErrorFormat(this, "[This HitBox is not part of a character, like a player, animal, etc, it has no purpose.", name);
				enabled = false;
				return;
			}

			m_Collider = GetComponent<Collider>();
			m_Rigidbody = GetComponent<Rigidbody>();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
        {
			gameObject.layer = LayerMask.NameToLayer("Hitbox"); 
		}
#endif
    }
}