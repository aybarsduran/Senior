using IdenticalStudios.InventorySystem;

namespace IdenticalStudios
{
    public interface IWorkstation
    {
        string WorkstationName { get; }
        IItemContainer[] GetContainers();
    }
}
