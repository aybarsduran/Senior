using IdenticalStudios.PoolingSystem;
using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Shooters/Hitscan Shooter")]
    public class FirearmHitscanShooter : FirearmShooterBehaviour
    {
        public override int AmmoPerShot => m_AmmoPerShot;

        [Title("Ammo")]

        [SerializeField, Range(0, 10)]
        private int m_AmmoPerShot = 1;

        [Title("Ray")]

        [SerializeField, Range(1, 30)]
        [Tooltip("The amount of rays that will be sent in the world")]
        private int m_RayCount = 1;

        [SerializeField, Range(0f, 100f)]
        private float m_MinSpread = 1f;

        [SerializeField, Range(0f, 100f)]
        private float m_MaxSpread = 2f;

        [SerializeField, Range(0f, 10000f)]
        private float m_MaxDistance = 300f;
        
        [SerializeField, Tooltip("The layers that will be affected when you fire.")]
        private LayerMask m_RayMask = (LayerMask)172545;

        [SerializeField]
        private QueryTriggerInteraction m_TriggerInteraction;

        [Title("Visual Effects")]

        [SerializeField, PrefabObjectOnly]
        private ParticleSystem m_TracerPrefab;

        [SerializeField, ReorderableList(HasLabels = false, Foldable = true)]
        private WieldableVFXBehaviour[] m_VisualEffects;

        [Title("Effects")]
        
        [SerializeField]
        private DynamicEffectCollection m_ShootEffects;
        
        [Title("Debug")]
        
        [SerializeField]
        private bool m_DebugHitscanRay;
        
        private RaycastHit m_HitInfo;
        
        
        public override void Shoot(float accuracy, IProjectileEffect effect, float triggerValue)
        {
            var character = Wieldable.Character;

            // Hitscan.
            float spread = Mathf.Lerp(m_MinSpread, m_MaxSpread, 1f - accuracy);
            for (int i = 0; i < m_RayCount; i++)
            {
                Ray ray = PhysicsUtils.GenerateRay(character.ViewTransform, spread);
                if (PhysicsUtils.RaycastNonAlloc(ray, m_MaxDistance, out m_HitInfo, m_RayMask, Wieldable.Character.Colliders, m_TriggerInteraction))
                {
                    effect.DoHitEffect(m_HitInfo, ray.direction, float.PositiveInfinity, m_HitInfo.distance, character);
#if UNITY_EDITOR
                    if (m_DebugHitscanRay)
                        Debug.DrawLine(ray.origin, m_HitInfo.point, Color.red, 5f);
#endif
                }

                PoolingManager.GetObject(m_TracerPrefab.gameObject, ray.origin, Quaternion.LookRotation(ray.direction), transform).GetComponent<ParticleSystem>().Play();
            }

            // Visual Effects.
            for (int i = 0; i < m_VisualEffects.Length; i++)
                m_VisualEffects[i].PlayEffect();

            // Effects.
            m_ShootEffects.PlayEffects(Wieldable, triggerValue);
        }

        protected override void Awake()
        {
            base.Awake();
            PoolingManager.CreatePool(m_TracerPrefab.gameObject, 2, 5);
        }
    }
}
