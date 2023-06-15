using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios
{
    public class CameraShakeManager : CameraShakeManagerBase
    {
        [SerializeField, NotNull]
        private AdditiveShakeMotionModule m_ShakeMotion;

        [SpaceArea()]

        [SerializeField]
        private ShakeSettings3D m_GetHitShake = ShakeSettings3D.Default;
        
        
        protected override void AddShake(ShakeSettings3D shakeSettings, float scale)
        {
            m_ShakeMotion.AddRotationShake(shakeSettings, scale);
        }
        
        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            Character.HealthManager.DamageTaken += OnDamageTaken;
        }

        protected override void OnBehaviourDisabled()
        {
            base.OnBehaviourDisabled();
            Character.HealthManager.DamageTaken -= OnDamageTaken;
        }

        private void OnDamageTaken(float damage)
        {
            if (damage < 5f)
                return;

            var healthManager = Character.HealthManager;
            float healthRemovePercent = Mathf.Min(1f, healthManager.MaxHealth / ((Character.HealthManager.PrevHealth - damage) * 2f));

            DoShake(m_GetHitShake, healthRemovePercent);
        }
    }
}