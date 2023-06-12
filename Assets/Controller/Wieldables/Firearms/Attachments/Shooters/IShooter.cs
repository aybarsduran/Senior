namespace IdenticalStudios.WieldableSystem
{
    public interface IShooter
    {
        int AmmoPerShot { get; }
        
        void Shoot(float accuracy, IProjectileEffect projectileEffect, float triggerValue);

        void Attach();
        void Detach();
    }
}