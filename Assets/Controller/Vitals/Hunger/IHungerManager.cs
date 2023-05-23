using IdenticalStudios;

namespace IdenticalStudios
{
    public interface IHungerManager : ICharacterModule
    {
        float Hunger { get; set; }
        float MaxHunger { get; set; }
    }
}