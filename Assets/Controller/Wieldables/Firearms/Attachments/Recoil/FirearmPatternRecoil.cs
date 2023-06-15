using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Recoil/Pattern Recoil")]
    public class FirearmPatternRecoil : FirearmRecoilBehaviour
    {
        #region Internal
        [Serializable]
        protected class RecoilData
        {
            public HeadRecoil HeadRecoil => m_HeadRecoil;
            public WieldableRecoil WieldableRecoil => m_WieldableRecoil;

            [SerializeField]
            private HeadRecoil m_HeadRecoil;

            [SerializeField]
            private WieldableRecoil m_WieldableRecoil;
        }

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
            public ShakeSettings3D HeadShake = ShakeSettings3D.Default;

            [Title("Pattern")]

            [Range(0.05f, 0.3f)]
            public float RecoveryDelay = 0.1f;
            public SpringSettings HeadPatternSpring = new SpringSettings(12, 100, 1, 1);
            public AnimCurves3D HeadPatternCurves;
        }

        #endregion

        [Title("Motion")]
        
        [SerializeField]
        private RecoilData m_HipfireRecoil;
        
        [SerializeField]
        private RecoilData m_AimedRecoil;

        private IMotionMixer m_WieldableMixer;
        private IMotionMixer m_HeadMixer;
        
        
        public override void DoRecoil(bool isAiming, float heatValue, float triggerValue)
        {
            float recoilMod = triggerValue * RecoilMultiplier;
            var data = isAiming ? m_AimedRecoil : m_HipfireRecoil;
            
            DoRecoil(data, heatValue, recoilMod);
        }

        private void DoRecoil(RecoilData data, float heatValue, float recoilMod)
        {
            // Head recoil
            var headRecoil = data.HeadRecoil;
            if (m_HeadMixer == null)
                m_HeadMixer = Wieldable.Character.GetModule<ICameraMotionHandler>().MotionMixer;

            if (m_HeadMixer.TryGetMotionOfType<CameraRecoilMotionModule>(out var headRecoilMotion))
            {
                var patternRecoil = headRecoil.HeadPatternCurves.Evaluate(heatValue) * 2f;
                headRecoilMotion.AddRecoil(patternRecoil, headRecoil.RecoveryDelay, headRecoil.HeadPatternSpring);
            }

            if (m_HeadMixer.TryGetMotionOfType<AdditiveForceMotionModule>(out var headForceMotion))
                headForceMotion.AddRotationForce(headRecoil.HeadForce, recoilMod);

            if (m_HeadMixer.TryGetMotionOfType<AdditiveShakeMotionModule>(out var headShakeMotion))
                headShakeMotion.AddRotationShake(headRecoil.HeadShake, recoilMod);

            // Wieldable recoil
            var wieldableRecoil = data.WieldableRecoil;

            if (m_WieldableMixer.TryGetMotionOfType<AdditiveForceMotionModule>(out var wieldableForceMotion))
            {
                wieldableForceMotion.AddPositionForce(wieldableRecoil.PositionForce, recoilMod, SpringType.FastSpring);
                wieldableForceMotion.AddRotationForce(wieldableRecoil.RotationForce, recoilMod, SpringType.FastSpring);
            }

            if (m_WieldableMixer.TryGetMotionOfType<AdditiveShakeMotionModule>(out var wieldableShakeMotion))
            {
                wieldableShakeMotion.AddPositionShake(wieldableRecoil.PositionShake, recoilMod);
                wieldableShakeMotion.AddRotationShake(wieldableRecoil.RotationShake, recoilMod);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_WieldableMixer = Wieldable.GetComponentInChildren<FPWieldableMotionMixer>(true);
        }
    }
}