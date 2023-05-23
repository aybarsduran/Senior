using System;

namespace IdenticalStudios.ProceduralMotion
{
    public abstract class MotionDataBase : ICloneable
    {
        public object Clone() => MemberwiseClone();
    }
}