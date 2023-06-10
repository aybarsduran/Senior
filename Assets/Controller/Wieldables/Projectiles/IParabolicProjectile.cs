using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface IParabolicProjectile
    {
	    GameObject gameObject { get; }
	    
	    event UnityAction onHit;
		
		void Launch(ICharacter character, Vector3 origin, Vector3 velocity, IProjectileEffect projectileEffect, LayerMask layers, float gravity, bool startImmediately = false);
	}
}