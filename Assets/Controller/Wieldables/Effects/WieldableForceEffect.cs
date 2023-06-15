using System;
using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class WieldableForceEffect : WieldableEffect
    {
        [SerializeField]
        private DelayedSpringForce3D m_PositionForce = DelayedSpringForce3D.Default;

        [SerializeField]
        private DelayedSpringForce3D m_RotationForce = DelayedSpringForce3D.Default;

        private AdditiveForceMotionModule m_ForceMotion;


        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_ForceMotion = wieldable.GetComponentInChildren<FPWieldableMotionMixer>(true).GetMotionOfType<AdditiveForceMotionModule>();
        }

        public override void PlayEffect()
        {
            m_ForceMotion.AddPositionForce(m_PositionForce);
            m_ForceMotion.AddRotationForce(m_RotationForce);
        }

        public override void PlayEffectDynamically(float value)
        {
            m_ForceMotion.AddPositionForce(m_PositionForce, value);
            m_ForceMotion.AddRotationForce(m_RotationForce, value);
        }
    }
}
