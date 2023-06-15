using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public abstract class OffsetMotionDataBase : MotionData
    {
        [Serializable]
        public struct OffsetData
        {
            [Range(0f, 10f)]
            public float Duration;

            public Vector3 PositionOffset;
            public Vector3 RotationOffset;
        }

        public abstract SpringSettings PositionSettings { get; }
        public abstract SpringSettings RotationSettings { get; }

        public abstract OffsetData[] Offsets { get; }

        public abstract SpringForce3D EnterForce { get; }
        public abstract SpringForce3D ExitForce { get; }
    }
}
