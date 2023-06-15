using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(false, null, "Assembly-CSharp", null)]
    public sealed class OffsetMotionData : OffsetMotionDataBase
    {
        public override SpringSettings PositionSettings => m_PositionSpring;
        public override SpringSettings RotationSettings => m_RotationSpring;

        public override OffsetData[] Offsets => m_Offsets;

        public override SpringForce3D EnterForce => m_EnterForce;
        public override SpringForce3D ExitForce => m_ExitForce;

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;
        
        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SpaceArea]

        [SerializeField]
        private SpringForce3D m_EnterForce;

        [SerializeField]
        private SpringForce3D m_ExitForce;

        [SpaceArea(spaceAfter: 5f)]

        [SerializeField, ReorderableList(ListStyle.Lined, "Offset", Foldable = false)]
        private OffsetData[] m_Offsets = Array.Empty<OffsetData>();
    }
}