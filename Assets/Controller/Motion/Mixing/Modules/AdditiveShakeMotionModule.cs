using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IdenticalStudios/Motion/Additive Shake Motion")]
    public sealed class AdditiveShakeMotionModule : MotionModule
    {
        [Title("Interpolation")]
        
        [SerializeField]
        private SpringSettings m_SlowPositionSpring = new SpringSettings(15f, 150f, 1.1f, 1f);

        [SerializeField]
        private SpringSettings m_SlowRotationSpring = new SpringSettings(15f, 150f, 1.1f, 1f);

        [SerializeField]
        private SpringSettings m_FastPositionSpring = new SpringSettings(15f, 150f, 1.1f, 1f);

        [SerializeField]
        private SpringSettings m_FastRotationSpring = new SpringSettings(15f, 150f, 1.1f, 1f);

        private readonly List<Shake3D> m_PositionShakes = new List<Shake3D>();
        private readonly List<Shake3D> m_RotationShakes = new List<Shake3D>();

        private SpringType m_PositionSpringMode = SpringType.FastSpring;
        private SpringType m_RotationSpringMode = SpringType.FastSpring;
        private const float k_PositionForceMod = 0.03f;
        private const float k_RotationForceMod = 3f;


        public void AddPositionShake(ShakeSettings3D shake, float scale = 1f, SpringType springType = SpringType.FastSpring)
        {
            if (shake.Duration < 0.01f)
                return;

            if (springType != m_PositionSpringMode)
            {
                m_PositionSpringMode = springType;
                positionSpring.Adjust(m_PositionSpringMode == SpringType.SlowSpring ? m_SlowPositionSpring : m_FastPositionSpring);
            }

            m_PositionShakes.Add(new Shake3D(shake, scale * k_PositionForceMod));
            SetTargetPosition(EvaluatePositionShakes());
        }

        public void AddRotationShake(ShakeSettings3D shake, float scale = 1f, SpringType springType = SpringType.FastSpring)
        {
            if (shake.Duration < 0.01f)
                return;

            if (springType != m_RotationSpringMode)
            {
                m_RotationSpringMode = springType;
                rotationSpring.Adjust(m_RotationSpringMode == SpringType.SlowSpring ? m_SlowRotationSpring : m_FastRotationSpring);
            }

            m_RotationShakes.Add(new Shake3D(shake, scale * k_RotationForceMod));
            SetTargetRotation(EvaluateRotationShakes());
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (positionSpring.IsIdle && rotationSpring.IsIdle)
                return;

            SetTargetPosition(EvaluatePositionShakes());
            SetTargetRotation(EvaluateRotationShakes());
        }

        protected override SpringSettings GetDefaultPositionSpringSettings() => m_FastPositionSpring;
        protected override SpringSettings GetDefaultRotationSpringSettings() => m_FastRotationSpring;

        private Vector3 EvaluatePositionShakes()
        {
            int i = 0;
            Vector3 value = default;

            while (i < m_PositionShakes.Count)
            {
                var shake = m_PositionShakes[i];
                value += shake.Evaluate();

                if (shake.IsDone)
                    m_PositionShakes.RemoveAt(i);
                else
                    i++;
            }

            return value;
        }

        private Vector3 EvaluateRotationShakes()
        {
            int i = 0;
            Vector3 value = default;

            while (i < m_RotationShakes.Count)
            {
                var shake = m_RotationShakes[i];
                value += shake.Evaluate();

                if (shake.IsDone)
                    m_RotationShakes.RemoveAt(i);
                else
                    i++;
            }

            return value;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                positionSpring.Adjust(m_FastPositionSpring);
                rotationSpring.Adjust(m_FastRotationSpring);
            }
        }
#endif
    }
}
