using UnityEngine;

namespace IdenticalStudios
{
    public interface ICharacterModule
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}