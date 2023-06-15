using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public abstract class BobMotionDataBase : MotionData
    {
        public enum BobMode
        {
            StepCycleBased,
            TimeBased
        }

        public abstract BobMode BobType { get; }
        public abstract float BobSpeed { get; }

        public abstract SpringSettings PositionSettings { get; }
        public abstract SpringSettings RotationSettings { get; }

        public abstract SpringForce3D PositionStepForce { get; }
        public abstract SpringForce3D RotationStepForce { get; }

        public abstract Vector3 PositionAmplitude { get; }
        public abstract Vector3 RotationAmplitude { get; }
    }
}
