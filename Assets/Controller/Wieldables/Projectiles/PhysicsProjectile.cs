using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public class PhysicsProjectile : PhysicsProjectileBase
    {
        private class CharacterEvent : UnityEvent<ICharacter> { }

        [SerializeField]
        private float m_DetonateDelay;

        [SerializeField]
        private UnityEvent m_DetonateEvent;

        [SerializeField]
        private ItemPickupBase m_ItemPickup;


        public override void LinkItem(IItem item)
        {
            if (m_ItemPickup != null)
                m_ItemPickup.LinkWithItem(item);
        }

        protected override void OnHit(Collision hit)
        {
            m_DetonateEvent.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_ItemPickup == null)
                m_ItemPickup = GetComponent<ItemPickupBase>();
        }
#endif
    }
}
