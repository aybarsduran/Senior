using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmProjectileEffectBehaviour : FirearmAttachmentBehaviour, IProjectileEffect
    {
        [SerializeField]
        protected DamageType m_DamageType = DamageType.Bullet;
        
        
        public abstract void DoHitEffect(RaycastHit hit, Vector3 hitDirection, float speed, float travelledDistance, ICharacter source);
        
        protected virtual void OnEnable() => Firearm?.SetBulletEffect(this);
        protected virtual void OnDisable() { }
    }
}