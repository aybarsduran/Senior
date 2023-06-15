using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface ITemperatureManager : ICharacterModule
    {
        float Temperature { get; set; }

        event UnityAction<float> TemperatureChanged;
    }
}