using IdenticalStudios.ProceduralMotion;
using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class FirearmBasicAimer : FirearmAimerBehaviour
    {
        public override float HipAccuracyMod => m_HipAccuracyMod;
        public override float AimAccuracyMod => m_AimAccuracyMod;


        [SerializeField, Range(0f, 1f)]
        private float m_HipAccuracyMod = 1f;
        
        [SerializeField, Range(0f, 1f)]
        private float m_AimAccuracyMod = 1f;

        [SerializeField, Range(0f, 10f)]
        private float m_HipRecoilMod = 1f;

        [SerializeField, Range(0f, 10f)]
        private float m_AimRecoilMod = 1f;


        [SerializeField, Range(0f, 2f)]
        protected float m_FOVSetDuration = 0.4f;

        [SerializeField, Range(0f, 2f)]
        protected float m_WorldFOVMod = 0.75f;

        [SerializeField, Range(0f, 2f)]
        protected float m_OverlayFOVMod = 0.75f;


        [SerializeField, Range(-1, 100)]
        protected int m_AimCrosshairIndex = 0;


        [SerializeField]
        private EffectCollection m_AimEffects = new(new IWieldableEffect[]
        {
            new WieldableMotionEnablerEffect(new OffsetMotionData()),
            new WieldableMotionWeightEffect()
        });

        [SerializeField]
        private EffectCollection m_AimEndEffects = new(new IWieldableEffect[]
        {
            new WieldableMotionDisablerEffect(typeof(OffsetMotionData)),
            new WieldableMotionWeightEffect()
        });

        private const float k_LookAndStrafeWeight = 0.35f;
        private const float k_NoiseAndBobWeight = 0.2f;
        private const float k_AimThreshold = 0.5f;

        private FPWieldableFOV m_FOVHandler;
        private FPWieldableMotionMixer m_MotionHandler;
        private ICrosshairHandler m_CrosshairHandler;
        private float m_NextPossibleAimTime;


        public override void StartAim()
        {
            if (IsAiming || Time.time < m_NextPossibleAimTime)
                return;

            // Field of View
            if (m_FOVHandler != null)
                HandleFOV(m_FOVHandler, true);

            // Crosshair
            m_CrosshairHandler.CrosshairIndex = m_AimCrosshairIndex;

            // Recoil multiplier
            Firearm?.Recoil.SetRecoilMultiplier(m_AimRecoilMod);

            m_MotionHandler.GetMotionOfType<NoiseMotionModule>().WeightMod = k_NoiseAndBobWeight;
            m_MotionHandler.GetMotionOfType<BobMotionModule>().WeightMod = k_NoiseAndBobWeight;

            m_MotionHandler.GetMotionOfType<LookMotionModule>().WeightMod = k_LookAndStrafeWeight;
            m_MotionHandler.GetMotionOfType<StrafeMotionModule>().WeightMod = k_LookAndStrafeWeight;

            m_MotionHandler.GetMotionOfType<ViewOffsetMotionModule>().WeightMod = 0f;

            IsAiming = true;
            
            // Effects
            m_AimEffects.PlayEffects(Wieldable);
        }

        public override void EndAim()
        {
            if (!IsAiming)
                return;

            m_NextPossibleAimTime = Time.time + k_AimThreshold;

            // Field of View
            if (m_FOVHandler != null)
                HandleFOV(m_FOVHandler, false);

            // Crosshair
            m_CrosshairHandler.ResetCrosshair();

            // Recoil multiplier
            Firearm?.Recoil.SetRecoilMultiplier(m_HipRecoilMod);

            m_MotionHandler.GetMotionOfType<NoiseMotionModule>().WeightMod = 1f;
            m_MotionHandler.GetMotionOfType<BobMotionModule>().WeightMod = 1f;

            m_MotionHandler.GetMotionOfType<LookMotionModule>().WeightMod = 1f;
            m_MotionHandler.GetMotionOfType<StrafeMotionModule>().WeightMod = 1f;

            m_MotionHandler.GetMotionOfType<ViewOffsetMotionModule>().WeightMod = 1f;

            IsAiming = false;
            
            // Effects
            m_AimEndEffects.PlayEffects(Wieldable);
        }

        protected virtual void HandleFOV(FPWieldableFOV fovHandler, bool aim)
        {
            if (aim)
            {
                fovHandler.SetOverlayFOV(m_OverlayFOVMod, m_FOVSetDuration * 1.1f);
                fovHandler.SetWorldFOV(m_WorldFOVMod, m_FOVSetDuration, 0.1f);
                fovHandler.SetDOFMod(1f);
            }
            else
            {
                fovHandler.SetOverlayFOV(1f, m_FOVSetDuration * 0.9f);
                fovHandler.SetWorldFOV(1f, m_FOVSetDuration * 0.9f);
                fovHandler.SetDOFMod(0f);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            m_CrosshairHandler = Wieldable as ICrosshairHandler;

            m_FOVHandler = Wieldable.gameObject.GetComponentInFirstChildren<FPWieldableFOV>();
            m_MotionHandler = Wieldable.gameObject.GetComponentInFirstChildren<FPWieldableMotionMixer>();
        }
    }
}
