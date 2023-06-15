using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IdenticalStudios/Motion/Additive Force Motion")]
    public sealed class AdditiveForceMotionModule : MotionModule
    {
        #region Internal
        private struct DistributedForce
        {
            public readonly Vector3 Force;
            public readonly float EndTime;

            public DistributedForce(Vector3 force, float duration)
            {
                Force = force;
                EndTime = Time.time + duration;
            }
        }
        #endregion

        [Title("Interpolation")]
        
        [SerializeField]
        private SpringSettings m_SlowPositionSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_SlowRotationSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_FastPositionSpring = SpringSettings.Default;

        [SerializeField]
        private SpringSettings m_FastRotationSpring = SpringSettings.Default;

        private readonly List<DistributedForce> m_PositionForces = new();
        private readonly List<DistributedForce> m_RotationForces = new();

        private SpringType m_PositionSpringMode = SpringType.SlowSpring;
        private SpringType m_RotationSpringMode = SpringType.SlowSpring;
        private const float k_PositionForceMod = 0.02f;


        public void AddPositionForce(SpringForce3D springForce, float scale = 1f, SpringType springType = SpringType.SlowSpring)
        {
            if (springForce.IsEmpty())
                return;

            if (springType != m_PositionSpringMode)
            {
                m_PositionSpringMode = springType;
                positionSpring.Adjust(m_PositionSpringMode == SpringType.SlowSpring ? m_SlowPositionSpring : m_FastPositionSpring);
            }

            Vector3 force = springForce.Force * (scale * k_PositionForceMod);
            m_PositionForces.Add(new DistributedForce(force, springForce.Duration));
            SetTargetPosition(EvaluatePositionForces());
        }

        public void AddRotationForce(SpringForce3D springForce, float scale = 1f, SpringType springType = SpringType.SlowSpring)
        {
            if (springForce.IsEmpty())
                return;

            if (springType != m_RotationSpringMode)
            {
                m_RotationSpringMode = springType;
                rotationSpring.Adjust(m_RotationSpringMode == SpringType.SlowSpring ? m_SlowRotationSpring : m_FastRotationSpring);
            }

            Vector3 force = springForce.Force * scale;
            m_RotationForces.Add(new DistributedForce(force, springForce.Duration));
            SetTargetRotation(EvaluateRotationForces());
        }

        public void AddPositionForce(DelayedSpringForce3D force, float scale = 1f, SpringType springType = SpringType.SlowSpring)
        {
            UpdateManager.InvokeDelayedAction(this, AddForce, force.Delay);
            void AddForce() => AddPositionForce(force.SpringForce, scale, springType);
        }
        
        public void AddRotationForce(DelayedSpringForce3D force, float scale = 1f, SpringType springType = SpringType.SlowSpring)
        {
            UpdateManager.InvokeDelayedAction(this, AddForce, force.Delay);
            void AddForce() => AddRotationForce(force.SpringForce, scale, springType);
        }

        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            
            positionSpring.Adjust(m_SlowPositionSpring);
            rotationSpring.Adjust(m_SlowRotationSpring);
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (positionSpring.IsIdle && rotationSpring.IsIdle)
                return;

            SetTargetPosition(EvaluatePositionForces());
            SetTargetRotation(EvaluateRotationForces());
        }

        private Vector3 EvaluatePositionForces()
        {
            int i = 0;
            float time = Time.time;
            Vector3 force = Vector3.zero;

            while (i < m_PositionForces.Count)
            {
                force += m_PositionForces[i].Force;

                if (time > m_PositionForces[i].EndTime)
                    m_PositionForces.RemoveAt(i);
                else
                    i++;
            }

            return force;
        }

        private Vector3 EvaluateRotationForces()
        {
            int i = 0;
            float time = Time.time;
            Vector3 force = Vector3.zero;

            while (i < m_RotationForces.Count)
            {
                force += m_RotationForces[i].Force;

                if (time > m_RotationForces[i].EndTime)
                    m_RotationForces.RemoveAt(i);
                else
                    i++;
            }

            return force;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                positionSpring.Adjust(m_SlowPositionSpring);
                rotationSpring.Adjust(m_SlowRotationSpring);
            }
        }
#endif
    }
}
