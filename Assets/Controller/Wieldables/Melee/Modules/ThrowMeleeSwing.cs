using IdenticalStudios.InventorySystem;
using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
	public class ThrowMeleeSwing : MeleeSwingBehaviour
	{
		[SerializeField]
		private ParabolicProjectileBase m_Projectile;

		[SerializeField]
		private FirearmProjectileEffectBehaviour m_ProjectileEffect;

		//Throw Settings

		[SerializeField, Range(0f, 100f)]
		private float m_MinSpread = 1f;
		
		[SerializeField, Range(0f, 100f)]
		private float m_MaxSpread = 1f;

		[SerializeField, Range(0f, 5f)]
		[Tooltip("Time to spawn the projectile")]
		private float m_SpawnDelay = 1f;

		[SerializeField]
		private Vector3 m_SpawnPositionOffset = Vector3.zero;

		[SerializeField]
		private Vector3 m_SpawnRotationOffset = Vector3.zero;

		[SerializeField, Range(0f, 1000f)]
		private float m_ThrowSpeed = 50f;
		
		//Miscellaneous

		[SerializeField, Range(0f, 100f)]
		[Tooltip("The gravity for the projectile.")]
		private float m_Gravity = 9.8f;
		
		[SerializeField]
		private LayerMask m_HitMask;

		[SerializeField]
		private bool m_RemoveFromInventory = true;


		[SerializeField, ReorderableList(HasLabels = false)]
		private Transform[] m_ObjectsToDisableOnThrow;

		//Effects

		[SerializeField]
		private EffectCollection m_ThrowStartEffects;

		[SerializeField]
		private EffectCollection m_ThrowEffects;

		private IAimInputHandler m_AimInputHandler;
		
		
        public override bool GetSwingValidity(float accuracy) => m_AimInputHandler != null && m_AimInputHandler.IsAiming;

        public override void DoSwing(float accuracy)
        {
	        // Effects
	        m_ThrowStartEffects.PlayEffects(Wieldable);
	        
			// Start the throw action with a delay.
			float spread = Mathf.Lerp(m_MinSpread, m_MaxSpread, 1f - accuracy);
			UpdateManager.InvokeDelayedAction(this, Throw, m_SpawnDelay);

			void Throw() => DoThrowSwing(PhysicsUtils.GenerateRay(Wieldable.Character.ViewTransform, spread));
        }

		public override void CancelSwing() => UpdateManager.StopAllDelayedActionsFor(this);

		private void DoThrowSwing(Ray ray)
		{
			ICharacter character = Wieldable.Character;
			
			Vector3 position = ray.origin + character.ViewTransform.TransformVector(m_SpawnPositionOffset);
			Quaternion rotation = Quaternion.LookRotation(ray.direction) * Quaternion.Euler(m_SpawnRotationOffset);

			IParabolicProjectile projectile = Instantiate(m_Projectile, position, rotation).GetComponent<IParabolicProjectile>();

			// Launch the projectile...
			if (projectile != null)
			{
				Vector3 throwVelocity = ray.direction * m_ThrowSpeed;

				if (character.TryGetModule(out ICharacterMotor motor))
					throwVelocity += motor.Velocity;
				
				projectile.Launch(character, ray.origin, throwVelocity, m_ProjectileEffect, m_HitMask, m_Gravity);

				if (projectile.gameObject.TryGetComponent(out ItemPickup pickup))
					pickup.LinkWithItem(Item.AttachedItem);
			}

			// Disable Objects
			foreach (var objectToDisable in m_ObjectsToDisableOnThrow)
				objectToDisable.localScale = Vector3.zero;
			
			// Remove Item from the user's inventory.
			if (m_RemoveFromInventory)
				character.Inventory.RemoveItem(Item.AttachedItem);
			
			// Effects.
			m_ThrowEffects.PlayEffects(Wieldable);
		}

		protected override void Awake()
		{
			base.Awake();

			m_AimInputHandler = GetComponent<IAimInputHandler>();
            Wieldable.EquippingStarted += OnEquipStart;
		}

        private void OnEquipStart()
        {
	        foreach (var objectToEnable in m_ObjectsToDisableOnThrow)
		        objectToEnable.localScale = Vector3.one;
        }
	}
}