namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmAimerBehaviour : FirearmAttachmentBehaviour, IAimer
    {
        public bool IsAiming { get; protected set; }

        public virtual float HipAccuracyMod => 1f;
        public virtual float AimAccuracyMod => 1f;
        
        
        public virtual void StartAim() => IsAiming = true;
        public virtual void EndAim() => IsAiming = false;

        protected virtual void OnEnable() => Firearm?.SetAimer(this);
        protected virtual void OnDisable() => IsAiming = false;
    }
}
