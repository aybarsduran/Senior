using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmAmmoBehaviour : FirearmAttachmentBehaviour, IAmmo
    {
        public event UnityAction<int> AmmoCountChanged;


        public virtual int RemoveAmmo(int amount) => 0;
        public virtual int AddAmmo(int amount) => 0;
        public virtual int GetAmmoCount() => 0;

        protected virtual void OnEnable() => Firearm?.SetAmmo(this);
        protected virtual void OnDisable() { }

        protected void RaiseAmmoChangedEvent(int ammo) => AmmoCountChanged?.Invoke(ammo);
    }
}