namespace IdenticalStudios.WieldableSystem
{
    public interface ICarriable : IWieldable
    {
        DataIdReference<CarriableDefinition> Definition { get; }

        int MaxCarryCount { get; }
        int CarryCount { get; }

        void InitializeCarriable(DataIdReference<CarriableDefinition> definition, ObjectDropper objectDropper);
        T GetCarriableActionOfType<T>() where T : CarriableActionBehaviour;

        bool TryAddCarriable(int count);
        bool TryDropCarriable(int count, float dropHeight);
        bool TryUseCarriable();
    }
}
