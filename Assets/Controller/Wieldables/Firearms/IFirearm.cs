using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface IFirearm
    {
        ICharacter Character { get; }
        
        IAimer Aimer { get; }
        ITrigger Trigger { get; }
        IShooter Shooter { get; }
        IAmmo Ammo { get; }
        IReloader Reloader { get; }
        IRecoil Recoil { get; }
        IProjectileEffect ProjectileEffect { get; }
        
        
        event UnityAction AttachmentChanged;
        event UnityAction<IAimer> AimerChanged;
        event UnityAction<ITrigger> TriggerChanged;
        event UnityAction<IShooter> ShooterChanged;
        event UnityAction<IAmmo> AmmoChanged;
        event UnityAction<IReloader> ReloaderChanged;
        event UnityAction<IRecoil> RecoilChanged;
        event UnityAction<IProjectileEffect> ProjectileEffectChanged;

        void SetAimer(IAimer aimer);
        void SetTrigger(ITrigger trigger);
        void SetShooter(IShooter shooter);
        void SetAmmo(IAmmo ammo);
        void SetReloader(IReloader reloader);
        void SetRecoil(IRecoil recoilHandler);
        void SetBulletEffect(IProjectileEffect projectileEffect);
    }
}