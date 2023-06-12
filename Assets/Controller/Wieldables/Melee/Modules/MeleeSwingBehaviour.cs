using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class MeleeSwingBehaviour : MonoBehaviour, IMeleeSwing
    {
		public virtual float SwingDuration => 1f;
		public virtual float AttackEffort => 0.1f;

		protected IWieldable Wieldable { get; private set; }
		protected IWieldableItem Item { get; private set; }

		private IItemProperty m_Durability;
		private static int s_DurabilityId = -1;
		
		
		public abstract bool GetSwingValidity(float accuracy);
		public abstract void DoSwing(float accuracy);
		public abstract void CancelSwing();

		protected virtual void Awake()
		{
			Wieldable = GetComponent<IWieldable>();
			Item = GetComponent<IWieldableItem>();

            Item.AttachedItemChanged += OnItemChanged;
		}

        private void OnItemChanged(IItem item)
        {
			m_Durability = null;

			if (item == null)
				return;

			if (s_DurabilityId == -1)
			{
				if (ItemPropertyDefinition.TryGetWithName("Durability", out var property))
					s_DurabilityId = property.Id;
				else
					s_DurabilityId = 0;
			}

			if (s_DurabilityId != 0)
				m_Durability = item.GetPropertyWithId(s_DurabilityId);
		}

        protected void ConsumeDurability(float amount)
		{
			// Lower the durability...
			if (m_Durability != null)
				m_Durability.Float = Mathf.Max(m_Durability.Float - amount, 0f);
		}
    }
}