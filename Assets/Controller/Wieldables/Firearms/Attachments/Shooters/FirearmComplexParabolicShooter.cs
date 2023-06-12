using IdenticalStudios.PoolingSystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class FirearmComplexParabolicShooter : FirearmParabolicShooter
    {
        [SerializeField, Range(0f, 10f)]
        private float m_InheritedVelocity = 0f;

        [SerializeField]
        private Vector3 m_SpawnPositionOffset = Vector3.zero;

        [SerializeField]
        private Vector3 m_SpawnRotationOffset = Vector3.zero;


        protected override void SpawnProjectile(Ray ray, IProjectileEffect effect, float speedMod)
        {
            var user = Wieldable.Character;

            Vector3 position = ray.origin + user.ViewTransform.TransformVector(m_SpawnPositionOffset);
            Quaternion rotation = Quaternion.LookRotation(ray.direction) * Quaternion.Euler(m_SpawnRotationOffset);

            IParabolicProjectile projectile = PoolingManager.GetObject(ProjectilePrefab.gameObject, position, rotation).GetComponent<IParabolicProjectile>();

            // Launch the projectile...
            Vector3 launchVelocity = ray.direction * (Speed * speedMod);

            if (user.TryGetModule(out ICharacterMotor motor))
                launchVelocity += motor.Velocity * m_InheritedVelocity;

            projectile.Launch(user, ray.origin, launchVelocity, effect, HitMask, Gravity);
        }
    }
}
