using UnityEngine;

namespace IdenticalStudios.ResourceGathering
{
    public sealed class TreeChopBehaviour : GatherableBehaviour, ISaveableComponent
    {
        #region Internal
        [System.Serializable]
        private class ChoppingSegment
        {
            public bool Enabled
            {
                get => m_Enabled;
                set
                {
                    m_Enabled = value;
                    m_Object.SetActive(m_Enabled);
                }
            }

            public GameObject Object 
            {
                get => m_Object;
                set => m_Object = value;
            }

            public Vector3 Normal => m_Normal;

            [SerializeField]
            private bool m_Enabled;

            private Vector3 m_Normal;
            private GameObject m_Object;


            public void Refresh() => m_Object.SetActive(m_Enabled);

            public ChoppingSegment(bool enabled, GameObject obj, Vector3 normal)
            {
                m_Enabled = enabled;
                m_Object = obj;
                m_Normal = normal;
            }
        }
        #endregion

        [SerializeField]
        [Tooltip("Center")]
        private Transform m_ChoppingPivot;

        [SerializeField]
        private Transform m_ChoppingSegmentsRoot;

        private ChoppingSegment[] m_ChoppingSegments;
        private Vector3 m_SegmentsCenter;
        private int m_ChoppedSegmentCount;


        public override void DoHitEffects(DamageContext dmgInfo)
        {
            if (m_ChoppingSegments == null)
            {
                m_ChoppingSegmentsRoot.gameObject.SetActive(true);
                m_ChoppingSegments = new ChoppingSegment[m_ChoppingSegmentsRoot.childCount];
                PrepareChoppingSegments();
            }

            int disabledSegments = m_ChoppingSegments.Length - (int)((Gatherable.Health / 100f) * m_ChoppingSegments.Length);
            int amountToChop = disabledSegments - m_ChoppedSegmentCount;

            if (amountToChop > 0)
            {
                for (int i = 0; i < amountToChop; i++)
                {
                    int indexToSelect = 0;

                    float largestAngle = 0f;
                    Vector3 chopPointNormal = (dmgInfo.HitPoint - m_SegmentsCenter).normalized;

                    for (int j = 0; j < m_ChoppingSegments.Length; j++)
                    {
                        if (!m_ChoppingSegments[j].Enabled)
                            continue;

                        float angle = Vector3.Angle(chopPointNormal, m_ChoppingSegments[j].Normal);

                        if (angle > largestAngle)
                        {
                            largestAngle = angle;
                            indexToSelect = j;
                        }
                    }

                    m_ChoppingSegments[indexToSelect].Enabled = false;
                }

                m_ChoppedSegmentCount += amountToChop;
            }
        }

        public override void DoDestroyEffects(DamageContext dmgInfo)
        {
            foreach (var segment in m_ChoppingSegments)
                segment.Enabled = false;
        }

        private void Awake()
        {
            m_ChoppingSegmentsRoot.gameObject.SetActive(false);
        }

        private void PrepareChoppingSegments()
        {
            m_SegmentsCenter = m_ChoppingPivot.position;

            int index = 0;

            foreach (Transform segmentObject in m_ChoppingSegmentsRoot)
            {
                ChoppingSegment segment = m_ChoppingSegments[index];

                var segmentNormal = (m_ChoppingPivot.position - segmentObject.position).normalized;

                if (segment == null)
                    segment = new ChoppingSegment(true, segmentObject.gameObject, segmentNormal);
                else
                {
                    segment.Object = segmentObject.gameObject;
                    segment.Refresh();
                }

                m_ChoppingSegments[index] = segment;

                index++;
            }
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_ChoppedSegmentCount = (int)members[0];
            m_ChoppingSegments = members[1] as ChoppingSegment[];

            if (m_ChoppingSegments != null)
                PrepareChoppingSegments();
        }

        public object[] SaveMembers()
        {
            return new object[] 
            {
                m_ChoppedSegmentCount,
                m_ChoppingSegments           
            };
        }
        #endregion
    }
}