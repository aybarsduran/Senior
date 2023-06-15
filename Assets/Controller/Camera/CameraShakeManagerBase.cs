using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios
{
    [RequireComponent(typeof(IMotionMixer))]
    public abstract class CameraShakeManagerBase : CharacterBehaviour
    {
        private static CameraShakeManagerBase s_Instance;
        
        
        public static void DoShake(Vector3 position, ShakeSettings3D shakeSettings, float radius, float scale = 1f)
        {
            if (s_Instance == null || shakeSettings.Duration < 0.01f)
                return;

            float distToImpactSqr = (s_Instance.transform.position - position).sqrMagnitude;
            float impactRadiusSqr = radius * radius;

            if (impactRadiusSqr - distToImpactSqr > 0f)
            {
                float distanceFactor = 1f - Mathf.Clamp01(distToImpactSqr / impactRadiusSqr);
                s_Instance.AddShake(shakeSettings, distanceFactor * scale);
            }
        }

        public static void DoShake(ShakeSettings3D shakeSettings, float scale = 1f)
        {
            if (s_Instance == null || shakeSettings.Duration < 0.01f)
                return;

            s_Instance.AddShake(shakeSettings, scale);
        }

        protected abstract void AddShake(ShakeSettings3D shakeSettings3D, float scale);

        protected override void OnBehaviourEnabled()
        {
            s_Instance = this;
        }
    }
}
