using IdenticalStudios;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IStaminaController : ICharacterModule
    {
        // Current stamina, 0 - 1 range
        float Stamina { get; set; }

        event UnityAction<float> StaminaChanged;
    }
}