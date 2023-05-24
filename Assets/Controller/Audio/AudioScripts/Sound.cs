using UnityEngine;

namespace IdenticalStudios
{
    public abstract class Sound
    {
        public abstract AudioClip AudioClip { get; }
        public abstract float Volume { get; }
        public abstract float Pitch { get; }
        public abstract float Delay { get; }
    }
}