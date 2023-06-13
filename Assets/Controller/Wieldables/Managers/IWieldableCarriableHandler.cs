using IdenticalStudios.WieldableSystem;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IWieldableCarriableHandler : ICharacterModule
    {
        int CarryCount { get; }
        ICarriable Carriable { get; }

        event UnityAction ObjectCarryStarted;
        event UnityAction ObjectCarryStopped;
        event UnityAction<int> CarriedCountChanged;


        bool TryCarryObject(DataIdReference<CarriableDefinition> definition);
        void DropCarriedObjects(int amount);
        void UseCarriedObject();
    }
}