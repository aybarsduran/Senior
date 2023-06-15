using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Recoil/Basic Recoil")]
    public class FirearmBasicRecoil : FirearmRecoilBehaviour
    {
        #region Internal
        [Serializable]
        protected class WieldableRecoil
        {
            public RandomSpringForce3D PositionForce;
            public ShakeSettings3D PositionShake;
            public RandomSpringForce3D RotationForce;
            public ShakeSettings3D RotationShake;
        }

        [Serializable]
        protected class HeadRecoil
        {
            public RandomSpringForce3D HeadForce = RandomSpringForce3D.Default;

            [FormerlySerializedAs("HeadShake")]
            public ShakeSettings3D HeadRotationShake = ShakeSettings3D.Default;

            public ShakeSettings3D HeadPositionShake = ShakeSettings3D.Default;
        }
        #endregion

        [SerializeField, Range(0f, 5f)]
        private float m_HipfireRecoilMod = 1f;

        [SerializeField, Range(0f, 5f)]
        private float m_AimRecoilMod = 0.7f;

        [SpaceArea]

        [SerializeField]
        private HeadRecoil m_HeadRecoil;

        [SerializeField]
        private WieldableRecoil m_WieldableRecoil;

        private IMotionMixer m_WieldableMixer;
        private IMotionMixer m_HeadMixer;


        public override void DoRecoil(bool isAiming, float heatValue, float triggerValue)
        {
            float recoilMod = triggerValue * RecoilMultiplier;
            recoilMod *= isAiming ? m_AimRecoilMod : m_HipfireRecoilMod;

            DoRecoil(recoilMod);
        }

        private void DoRecoil(float recoilMod)
        {
            // Head recoil
            if (m_HeadMixer == null)
                m_HeadMixer = Wieldable.Character.GetModule<ICameraMotionHandler>().MotionMixer;

            if (m_HeadMixer.TryGetMotionOfType<AdditiveForceMotionModule>(out var headForceMotion))
                headForceMotion.AddRotationForce(m_HeadRecoil.HeadForce, recoilMod, SpringType.FastSpring);

            if (m_HeadMixer.TryGetMotionOfType<AdditiveShakeMotionModule>(out var headShakeMotion))
            {
                headShakeMotion.AddPositionShake(m_HeadRecoil.HeadPositionShake, recoilMod, SpringType.FastSpring);
                headShakeMotion.AddRotationShake(m_HeadRecoil.HeadRotationShake, recoilMod, SpringType.FastSpring);
            }

            // Wieldable recoil
            if (m_WieldableMixer.TryGetMotionOfType<AdditiveForceMotionModule>(out var wieldableForceMotion))
            {
                wieldableForceMotion.AddPositionForce(m_WieldableRecoil.PositionForce, recoilMod, SpringType.FastSpring);
                wieldableForceMotion.AddRotationForce(m_WieldableRecoil.RotationForce, recoilMod, SpringType.FastSpring);
            }

            if (m_WieldableMixer.TryGetMotionOfType<AdditiveShakeMotionModule>(out var wieldableShakeMotion))
            {
                wieldableShakeMotion.AddPositionShake(m_WieldableRecoil.PositionShake, recoilMod, SpringType.FastSpring);
                wieldableShakeMotion.AddRotationShake(m_WieldableRecoil.RotationShake, recoilMod, SpringType.FastSpring);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_WieldableMixer = Wieldable.GetComponentInChildren<FPWieldableMotionMixer>(true);
        }
    }
}
