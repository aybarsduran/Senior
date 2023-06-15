using System;
using UnityEngine;
using IdenticalStudios.ProceduralMotion;

namespace IdenticalStudios.WieldableSystem
{
    [Serializable]
    public class RecoilMotionData
    {
        public SpringSettings PositionSettings => m_PositionSpring;
        public SpringSettings RotationSettings => m_RotationSpring;

        public AnimCurves2D PositionCurves => m_PositionCurves;
        public AnimCurves2D RotationCurves => m_RotationCurves;

        public SpringForce3D PositionForce => m_PositionForce;
        public SpringForce3D RotationForce => m_RotationForce;
        public ShakeSettings3D RecoilShake => m_RotationShake;

        [Title("Spring Settings")]

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [Title("Recoil Pattern")]

        [SerializeField]
        private AnimCurves2D m_PositionCurves;

        [SerializeField]
        private AnimCurves2D m_RotationCurves;

        [Title("Recoil Forces")]

        [SerializeField]
        private SpringForce3D m_PositionForce = SpringForce3D.Default;

        [SerializeField]
        private SpringForce3D m_RotationForce = SpringForce3D.Default;

        [SerializeField]
        private ShakeSettings3D m_RotationShake = ShakeSettings3D.Default;
    }
}