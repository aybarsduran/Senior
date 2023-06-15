using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IThirstManager : ICharacterModule
    {
        float Thirst { get; set; }
        float MaxThirst { get; set;  }
    }
}