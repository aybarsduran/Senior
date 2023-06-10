using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface IPhysicsProjectile
    {
        GameObject gameObject { get; }

        event UnityAction onHit;

        void Launch(ICharacter character, Vector3 origin, Vector3 velocity);
        void LinkItem(IItem item);
    }
}
