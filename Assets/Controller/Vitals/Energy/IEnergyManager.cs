namespace IdenticalStudios
{
    public interface IEnergyManager : ICharacterModule
    {
        float Energy { get; set; }
        float MaxEnergy { get; set; }
    }
}