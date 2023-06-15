using System;

namespace IdenticalStudios.ProceduralMotion
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public abstract class MotionDataBase : ICloneable
    {
        public object Clone() => MemberwiseClone();
    }
}