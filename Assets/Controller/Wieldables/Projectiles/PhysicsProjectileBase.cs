using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PhysicsProjectileBase : MonoBehaviour, IPhysicsProjectile
    {
        protected ICharacter Character => m_Character;
        protected bool InAir => m_InAir;

        public event UnityAction onHit;

        [SerializeField, NotNull]
        private Rigidbody m_Rigidbody;

        private ICharacter m_Character;
        private bool m_InAir;
        private bool m_Hit;


        public void Launch(ICharacter character, Vector3 origin, Vector3 velocity)
        {
            m_Character = character;
            m_InAir = true;
            m_Hit = false;

            m_Rigidbody.isKinematic = false;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            m_Rigidbody.velocity = velocity;
        }

        public virtual void LinkItem(IItem item) { }

        protected virtual void OnHit(Collision hit) { }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (m_Hit)
                return;

            m_Hit = true;
            m_Rigidbody.interpolation = RigidbodyInterpolation.None;

            onHit?.Invoke();
            OnHit(collision);
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
#endif
    }
}
