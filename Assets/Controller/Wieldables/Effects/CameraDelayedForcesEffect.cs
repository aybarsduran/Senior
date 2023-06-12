using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class CameraDelayedForcesEffect : WieldableEffect
    {
        [SerializeField, ReorderableList(ListStyle.Boxed, childLabel: "Force")]
        private DelayedSpringForce3D[] m_CameraForces;

        private AdditiveForceMotionModule m_ForceMotion;


        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_ForceMotion = wieldable.Character.GetModule<ICameraMotionHandler>().MotionMixer
                .GetMotionOfType<AdditiveForceMotionModule>();
        }

        public override void PlayEffect()
        {
            for (int i = 0; i < m_CameraForces.Length; i++)
                m_ForceMotion.AddRotationForce(m_CameraForces[i]);
        }

        public override void PlayEffectDynamically( float value)
        {
            for (int i = 0; i < m_CameraForces.Length; i++)
                m_ForceMotion.AddRotationForce(m_CameraForces[i], value);
        }
    }
}