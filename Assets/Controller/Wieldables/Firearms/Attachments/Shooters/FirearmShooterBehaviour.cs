namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmShooterBehaviour : FirearmAttachmentBehaviour, IShooter
    {
        public virtual int AmmoPerShot => 1;
        
        
        public abstract void Shoot(float accuracy, IProjectileEffect projectileEffect, float value);
        protected virtual void OnEnable() => Firearm?.SetShooter(this);
    }
}
