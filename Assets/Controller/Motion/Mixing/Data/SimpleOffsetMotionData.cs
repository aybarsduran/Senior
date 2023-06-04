using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public class SimpleOffsetMotionData : OffsetMotionDataBase
    {
        public override SpringSettings PositionSettings => SpringSettings.Default;
        public override SpringSettings RotationSettings => SpringSettings.Default;

        public override OffsetData[] Offsets => m_Offsets;

        public override SpringForce3D EnterForce => SpringForce3D.Default;
        public override SpringForce3D ExitForce => SpringForce3D.Default;

        [SerializeField, ReorderableList(ListStyle.Lined, "Offset", Foldable = false)]
        private OffsetData[] m_Offsets = Array.Empty<OffsetData>();
    }
}
