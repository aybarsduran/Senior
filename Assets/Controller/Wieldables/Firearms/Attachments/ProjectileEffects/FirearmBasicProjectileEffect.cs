using IdenticalStudios.Surfaces;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/BulletEffects/Basic Projectile Effect")]
    public class FirearmBasicProjectileEffect : FirearmProjectileEffectBehaviour
    {
        [SerializeField, Range(0f, 1000f)]
        [Tooltip("The damage at close range.")]
        private float m_Damage = 15f;
        
        [SerializeField, Range(0f, 1000f)]
        [Tooltip("The impact impulse that will be transferred to the rigidbodies at contact.")]
        private float m_Force = 15f;
        
        
        public override void DoHitEffect(RaycastHit hit, Vector3 hitDirection, float speed, float travelledDistance, ICharacter source)
        {
            // Apply an impact impulse
            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(hitDirection * m_Force, hit.point, ForceMode.Impulse);
            
            if (hit.collider.TryGetComponent(out IDamageReceiver receiver))
                receiver.HandleDamage(m_Damage, new DamageContext(m_DamageType, hit.point, hitDirection * m_Force, hit.normal, Wieldable.Character));
            
            SurfaceManager.SpawnEffect(hit, SurfaceEffects.BulletHit, 1f, true);
        }
    }
}