using IdenticalStudios.PoolingSystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class ParabolicProjectile : ParabolicProjectileBase
    {
        [SerializeField]
        private TrailRenderer m_TrailRenderer;

        [SerializeField]
        private ParticleSystem m_ParticleSystem;

        [SerializeField]
        private PoolableObject m_PoolableObject;

        private int m_FrameSinceLaunched;
        
        
        protected override void OnLaunched()
        {
            if (m_ParticleSystem != null)
                m_ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (m_TrailRenderer != null)
            {
                m_TrailRenderer.time = 0f;
                m_TrailRenderer.emitting = false;
                m_TrailRenderer.Clear();
            }

            m_FrameSinceLaunched = 0;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!InAir)
                return;

            if (m_FrameSinceLaunched == 1)
            {
                if (m_ParticleSystem != null)
                    m_ParticleSystem.Play();

                if (m_TrailRenderer != null)
                {
                    m_TrailRenderer.emitting = true;
                    m_TrailRenderer.time = 0.125f;
                }
            }

            m_FrameSinceLaunched++;
        }

        protected override void OnHit(RaycastHit hit)
        {
            if (m_PoolableObject != null)
            {
                m_PoolableObject.ReleaseObject(0.5f);
                return;
            }

            // Clean the trail.
            if (m_TrailRenderer != null)
                m_TrailRenderer.emitting = false;

            if (m_ParticleSystem != null)
                m_ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_PoolableObject == null)
                m_PoolableObject = GetComponent<PoolableObject>();
        }
#endif
    }
}
