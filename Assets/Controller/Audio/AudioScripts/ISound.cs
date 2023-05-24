using System;
using UnityEngine;

namespace IdenticalStudios
{
    public interface ISound
    {
        AudioClip AudioClip { get; }
        float Volume { get; }
        float Pitch { get; }
        float Delay { get; }
    }
}