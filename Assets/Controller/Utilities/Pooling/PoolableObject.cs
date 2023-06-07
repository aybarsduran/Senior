using UnityEngine;

namespace IdenticalStudios.PoolingSystem
{
    public class PoolableObject : MonoBehaviour
    {
        public int PoolId => m_PoolId;
        public float AutoReleaseDelay
        {
            get => m_AutoReleaseDelay;
            set
            {
                m_AutoReleaseDelay = value;
                m_TimeToRelease = value;
            }
        }

        [SerializeField, HideInInspector]
        private int m_PoolId = -1;

        [SerializeField, Range(0f, 1000f)]
        private float m_AutoReleaseDelay;

        private float m_TimeToRelease;


        public void Init(int poolId)
        {
            if (PoolId != -1)
            {
                Debug.LogError("You are attempting to initialize a poolable object, but it's already initialized!!");
                return;
            }

            m_PoolId = poolId;
        }

        public void ReleaseObject(float delay) => m_TimeToRelease = delay;

        public void ReleaseObject()
        {
            var pool = PoolingManager.GetPool(PoolId);
            pool?.ReleaseInstance(this);
        }

        private void OnEnable()
        {
            m_TimeToRelease = m_AutoReleaseDelay < 0.001f ? float.MaxValue : Time.time + m_AutoReleaseDelay;
        }

        private void Update()
        {
            if (PoolId == -1)
                return;

            if (m_TimeToRelease < Time.time)
                ReleaseObject();
        }
    }
}