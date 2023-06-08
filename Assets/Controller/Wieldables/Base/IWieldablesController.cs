using IdenticalStudios.WieldableSystem;
using IdenticalStudios;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IWieldablesController : ICharacterModule
    {
        IWieldable ActiveWieldable { get; }
        bool IsEquipping { get; }

        event WieldableEquipDelegate WieldableHolsterStarted;
        event WieldableEquipDelegate WieldableEquipStarted;
        event WieldableEquipDelegate WieldableEquipStopped;

        bool GetWieldableOfType<T>(out T wieldable) where T : IWieldable;
        bool HasWieldable(IWieldable wieldable);

        bool TryEquipWieldable(IWieldable wieldable, float holsterSpeed = 1f, UnityAction equippedCallback = null);

        IWieldable AddWieldable(IWieldable wieldable, bool spawn = true, bool disable = true);
        bool RemoveWieldable(IWieldable wieldable, bool destroy = false);
    }

    public delegate void WieldableEquipDelegate(IWieldable equippedWieldable);
}