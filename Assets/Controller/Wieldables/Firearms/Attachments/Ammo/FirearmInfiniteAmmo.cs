using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Ammo/Infinite Ammo")]
    public class FirearmInfiniteAmmo : FirearmAmmoBehaviour
    {
        public override int RemoveAmmo(int amount) => amount;
        public override int AddAmmo(int amount) => amount;
        public override int GetAmmoCount() => int.MaxValue;
    }
}