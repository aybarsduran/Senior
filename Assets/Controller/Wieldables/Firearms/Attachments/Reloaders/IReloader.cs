using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface IReloader
    {
        bool IsReloading { get; }

        int AmmoInMagazine { get; set; }
        int AmmoToLoad { get; }
        int MagazineSize { get; }
        bool IsMagazineFull { get; }
        bool IsMagazineEmpty { get; }

        /// <summary> Prev ammo, Current ammo </summary>
        event UnityAction<int, int> AmmoInMagazineChanged;
        event UnityAction ReloadStart;
        event UnityAction ReloadFinish;


        bool TryStartReload(IAmmo ammoModule);
        bool TryCancelReload(IAmmo ammoModule, out float endDuration);
        bool TryUseAmmo(int amount);

        void Attach();
        void Detach();
    }
}