using IdenticalStudios.PoolingSystem;
using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Shooters/Parabolic Shooter")]
    public class FirearmParabolicShooter : FirearmShooterBehaviour
    {
        public override int AmmoPerShot => m_AmmoPerShot;
        protected ParabolicProjectileBase ProjectilePrefab => m_Projectile;
        protected LayerMask HitMask => m_HitMask;
        protected float Gravity => m_Gravity;
        protected float Speed => m_Speed;

        [SerializeField, Range(0, 10)]
        private int m_AmmoPerShot = 1;

        [SerializeField]
        private LayerMask m_HitMask = Physics.DefaultRaycastLayers;
        
        [SerializeField]
        private ParabolicProjectileBase m_Projectile;

        [SerializeField, Range(1, 30)] 
        [Tooltip("The amount of projectiles that will be spawned in the world")]
        private int m_Count = 1;

        [SerializeField, Range(1f, 1000f)]
        private float m_Speed = 75f;

        [SerializeField, Range(0f, 100f)]
        [Tooltip("The gravity for the projectile.")]
        private float m_Gravity = 9.8f;

        [SerializeField, Range(0f, 100f)]
        private float m_MinSpread = 0.75f;

        [SerializeField, Range(0f, 100f)]
        private float m_MaxSpread = 1.5f;
        
        [SerializeField]
        private DynamicEffectCollection m_ShootEffects;

        [SerializeField]
        private WieldableVFXBehaviour[] m_VisualEffects;


        public override void Shoot(float accuracy, IProjectileEffect effect, float value)
        {
            ICharacter character = Wieldable.Character;
            
            // Spawn Projectile(s).
            float spread = Mathf.Lerp(m_MinSpread, m_MaxSpread, 1f - accuracy);
            for (int i = 0; i < m_Count; i++)
            {
                Ray ray = PhysicsUtils.GenerateRay(character.ViewTransform, spread);
                SpawnProjectile(ray, effect, value);
            }

            // Visual Effects.
            for (int i = 0; i < m_VisualEffects.Length; i++)
                m_VisualEffects[i].PlayEffect();

            // Effects.
            m_ShootEffects.PlayEffects(Wieldable, value);
        }

        protected virtual void SpawnProjectile(Ray ray, IProjectileEffect effect, float speedMod)
        {
            var projectile = PoolingManager.GetObject(m_Projectile.gameObject, ray.origin, Quaternion.LookRotation(ray.direction)).GetComponent<IParabolicProjectile>();
            projectile.Launch(Wieldable.Character, ray.origin, ray.direction * (m_Speed * speedMod), effect, m_HitMask, m_Gravity);
        }

        protected override void Awake()
        {
            base.Awake();
            PoolingManager.CreatePool(m_Projectile.gameObject, 3, 10, 120f);
        }
    }
}
