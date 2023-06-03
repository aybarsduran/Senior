using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [RequireComponent(typeof(IMotionMixer))]
    public abstract class MotionModule : CharacterBehaviour, IMixedMotion
    {
        IMotionMixer IMixedMotion.MotionMixer => motionMixer;
        public float WeightMod { get => m_WeightMod; set => m_WeightMod = value; }
        protected float Weight => m_Weight * m_WeightMod;

        [SerializeField, Range(0f, 1f)]
        private float m_Weight = 1f;

        private float m_WeightMod = 1f;
        protected IMotionMixer motionMixer;
        protected readonly Spring3D positionSpring = new();
        protected readonly Spring3D rotationSpring = new();


        public abstract void TickMotionLogic(float deltaTime);

        public Vector3 GetPosition(float deltaTime)
        {
            Vector3 value = positionSpring.Evaluate(deltaTime);
            return value;
        }

        public Quaternion GetRotation(float deltaTime)
        {
            Vector3 value = rotationSpring.Evaluate(deltaTime);
            return Quaternion.Euler(value);
        }

        protected void SetTargetPosition(Vector3 target)
        {
            target *= m_Weight * m_WeightMod;
            positionSpring.SetTargetValue(target);
        }

        protected void SetTargetPosition(Vector3 target, float multiplier = 1f)
        {
            target *= m_Weight * m_WeightMod * multiplier;
            positionSpring.SetTargetValue(target);
        }

        protected void SetTargetRotation(Vector3 target)
        {
            target *= m_Weight * m_WeightMod;
            rotationSpring.SetTargetValue(target);
        }

        protected void SetTargetRotation(Vector3 target, float multiplier = 1f)
        {
            target *= m_Weight * m_WeightMod * multiplier;
            rotationSpring.SetTargetValue(target);
        }

        protected override void OnBehaviourEnabled()
        {
            positionSpring.Adjust(GetDefaultPositionSpringSettings());
            rotationSpring.Adjust(GetDefaultRotationSpringSettings());

            if (motionMixer == null)
                motionMixer = GetComponent<IMotionMixer>();

            motionMixer.AddMixedMotion(this);
        }

        protected override void OnBehaviourDisabled()
        {
            motionMixer.RemoveMixedMotion(this);
        }

        protected virtual SpringSettings GetDefaultPositionSpringSettings() => SpringSettings.Default;
        protected virtual SpringSettings GetDefaultRotationSpringSettings() => SpringSettings.Default;
    }
}
