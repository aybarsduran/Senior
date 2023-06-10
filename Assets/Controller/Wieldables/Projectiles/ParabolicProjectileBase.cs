using IdenticalStudios.WieldableSystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public abstract class ParabolicProjectileBase : MonoBehaviour, IParabolicProjectile
    {
#if UNITY_EDITOR
        protected Transform CachedTransform { get; private set; }
        protected ICharacter Character { get; private set; }
        protected bool InAir { get; private set; }
#else
        protected Transform CachedTransform;
        protected ICharacter Character;
        protected bool InAir;
#endif

        public event UnityAction onHit;

        [SerializeField]
        private bool m_MatchRotation;

        [SerializeField]
        private QueryTriggerInteraction m_QueryTriggers;
        
        private IProjectileEffect m_ProjectileEffect;
        private float m_Speed;
        private float m_Gravity;
        private Vector3 m_StartPosition;
        private Vector3 m_StartDirection;
        private Vector3 m_LerpTargetPos;
        private Vector3 m_LerpStartPos;
        private float m_StartTime;
        private RaycastHit m_Hit;
        private LayerMask m_Layers;
        
        
        public void Launch(ICharacter character, Vector3 origin, Vector3 velocity, IProjectileEffect projectileEffect, LayerMask layers, float gravity, bool startImmediately = false)
        {
            m_StartPosition = origin;
            m_LerpStartPos = origin;
            m_LerpTargetPos = origin;
            m_StartDirection = velocity.normalized;
            m_Speed = velocity.magnitude;
            m_Gravity = gravity;

            Character = character;
            m_ProjectileEffect = projectileEffect;
            m_Layers = layers;

            m_StartTime = -1f;

            InAir = true;
            OnLaunched();

            if (startImmediately)
                FixedUpdate();
        }

        protected void Relaunch(float speed)
        {
            if (m_Hit.collider == null || Character == null)
                return;

            Vector3 origin = CachedTransform.position;
            Vector3 velocity = CachedTransform.forward * speed;
            Launch(Character, origin, velocity, m_ProjectileEffect, m_Layers, m_Gravity, true);
        }

        protected virtual void OnLaunched() { }
        protected virtual void OnHit(RaycastHit hit) { }

        protected virtual void FixedUpdate()
        {
            if (!InAir)
                return;

            if (m_StartTime < 0f)
                m_StartTime = Time.time;

            float currentTime = Time.time - m_StartTime;
            float nextTime = currentTime + Time.fixedDeltaTime;

            Vector3 currentPoint = EvaluateParabola(currentTime);
            Vector3 nextPoint = EvaluateParabola(nextTime);

            Vector3 direction = nextPoint - currentPoint;
            float distance = direction.magnitude;
            var ray = new Ray(currentPoint, direction);

            if (PhysicsUtils.RaycastNonAlloc(ray, distance, out m_Hit, m_Layers, Character.Colliders, m_QueryTriggers))
            {
                m_ProjectileEffect?.DoHitEffect(m_Hit, ray.direction, direction.magnitude, (m_StartPosition - m_Hit.point).magnitude, Character);

                InAir = false;
                onHit?.Invoke();
                OnHit(m_Hit);
            }
            else
            {
                m_LerpStartPos = currentPoint;
                m_LerpTargetPos = nextPoint;
            }
        }

        protected virtual Vector3 EvaluateParabola(float time)
        {
            Vector3 point = m_StartPosition + (m_StartDirection * (m_Speed * time));
            Vector3 gravity = Vector3.down * (m_Gravity * time * time);
            return point + gravity;
        }

        protected virtual void Update()
        {
            if (!InAir)
                return;

            float delta = Time.unscaledTime - Time.fixedUnscaledTime;
            if (delta < Time.fixedUnscaledDeltaTime)
            {
                float t = delta / Time.fixedUnscaledDeltaTime;
                CachedTransform.localPosition = Vector3.Lerp(m_LerpStartPos, m_LerpTargetPos, t);
            }
            else
                CachedTransform.localPosition = m_LerpTargetPos;

            if (m_MatchRotation)
            {
                var velocity = m_LerpTargetPos - m_LerpStartPos;

                if (velocity != Vector3.zero)
                    CachedTransform.rotation = Quaternion.LookRotation(velocity);
            }
        }
        
        protected virtual void Awake()
        {
            CachedTransform = transform;
        }
    }
}
