using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class Throwable : Wieldable, IUseInputHandler
    {
        public ActionBlockHandler UseBlocker { get; private set; } = new ActionBlockHandler();
        public Sprite DisplayIcon => m_DisplayIcon;

		private enum ItemInteractionMode
		{
			Default,
			RemoveAndLinkToPickup
		}

		[SerializeField]
		private Sprite m_DisplayIcon;

		[SpaceArea]

		[SerializeField, NotNull]
        private PhysicsProjectileBase m_Projectile;

		[SerializeField, Range(0f, 5f)]
		private float m_ThrowDelay;

		[SerializeField]
		private Vector3 m_PositionOffset;

		[SerializeField]
		private Vector3 m_RotationOffset;

		[SerializeField, Range(0f, 100f)]
		private float m_ThrowForce = 50f;

		[SerializeField]
		private ItemInteractionMode m_ItemInteraction = ItemInteractionMode.RemoveAndLinkToPickup;

		[SpaceArea]

		[SerializeField, ReorderableList(HasLabels = false)]
		private Transform[] m_ObjectsToDisableOnThrow;

		[Title("Effects")]

		[SerializeField]
		private EffectCollection m_ThrowStartEffects;

		[SerializeField]
		private EffectCollection m_ThrowEffects;

		private IWieldableItem m_WieldableItem;


		public void Use(UsePhase usePhase)
        {
			// Effects
			m_ThrowStartEffects.PlayEffects(this);

			// Start the throw action with a delay.
			UpdateManager.InvokeDelayedAction(this, Throw, m_ThrowDelay);
		}

        public override void OnEquip()
        {
            base.OnEquip();

			// Enable Objects
			foreach (var objectToEnable in m_ObjectsToDisableOnThrow)
				objectToEnable.localScale = Vector3.one;
		}

        protected override void Awake()
		{
			base.Awake();
			m_WieldableItem = GetComponent<IWieldableItem>();
		}

		private void Throw()
		{
			Ray ray = PhysicsUtils.GenerateRay(Character.ViewTransform, AccuracyHandler.GetAccuracyMod(), m_PositionOffset);

			Vector3 position = ray.origin;
			Quaternion rotation = Quaternion.LookRotation(ray.direction) * Quaternion.Euler(m_RotationOffset);

			var projectile = Instantiate(m_Projectile, position, rotation);

			// Launch the projectile...
			Vector3 throwVelocity = ray.direction * m_ThrowForce;

			if (Character.TryGetModule(out ICharacterMotor motor))
				throwVelocity += motor.Velocity;

			projectile.Launch(Character, ray.origin, throwVelocity);

			// Remove from inventory and link to pickup.
			if (m_ItemInteraction == ItemInteractionMode.RemoveAndLinkToPickup && m_WieldableItem != null)
			{
				if (Character.Inventory.RemoveItem(m_WieldableItem.AttachedItem))
					projectile.LinkItem(m_WieldableItem.AttachedItem);
			}

			// Disable Objects
			foreach (var objectToDisable in m_ObjectsToDisableOnThrow)
				objectToDisable.localScale = Vector3.zero;

			// Effects.
			m_ThrowEffects.PlayEffects(this);
		}
    }
}
