using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface IAmmo
    {
        event UnityAction<int> AmmoCountChanged;

        int RemoveAmmo(int amount);
        int AddAmmo(int amount);
        int GetAmmoCount();

        void Attach();
        void Detach();
    }
}