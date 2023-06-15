using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmRecoilBehaviour : FirearmAttachmentBehaviour, IRecoil
    {
        public float RecoilHeatRecover => m_RecoilHeatRecover;
        public float HipfireAccuracyKick => m_HipfireAccuracyKick;
        public float HipfireAccuracyRecover => m_HipfireAccuracyRecover;
        public float AimAccuracyKick => m_AimAccuracyKick;
        public float AimAccuracyRecover => m_AimAccuracyRecover;

        protected virtual float RecoilMultiplier { get; private set; } = 1f;

        [SpaceArea]
        
        [SerializeField, Range(0f, 1f)]
        private float m_HipfireAccuracyKick = 0.075f;

        [SerializeField, Range(0f, 1f)]
        private float m_HipfireAccuracyRecover = 0.2f;

        [SerializeField, Range(0f, 1f)]
        private float m_AimAccuracyKick = 0.05f;

        [SerializeField, Range(0f, 1f)]
        private float m_AimAccuracyRecover = 0.275f;

        [SerializeField, Range(0f, 1f)]
        private float m_RecoilHeatRecover = 0.35f;
        
        
        public abstract void DoRecoil(bool isAiming, float heatValue, float triggerValue);
        public virtual void SetRecoilMultiplier(float multiplier) => RecoilMultiplier = multiplier;

        protected virtual void OnEnable() => Firearm?.SetRecoil(this);
        protected virtual void OnDisable() { }
    }
}