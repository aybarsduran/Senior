using System.Collections;
using System.Collections.Generic;
using IdenticalStudios;
using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios
{
    public interface IWieldableMotionHandler : ICharacterModule
    {
        IMotionMixer MotionMixer { get; }
        IMotionDataHandler DataHandler { get; }
    }
}
