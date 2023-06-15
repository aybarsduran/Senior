using IdenticalStudios.ProceduralMotion;
using System.Collections;
using UnityEngine;

namespace IdenticalStudios.ResourceGathering
{
    public sealed class TreeFallBehaviour : GatherableBehaviour, ISaveableComponent
    {
        [Title("References")]

        [SerializeField, NotNull]
        private Rigidbody m_FallingTree;

        [SerializeField, NotNull]
        private GameObject m_TreeStump;

        [SerializeField, NotNull]
        private ColliderTriggerHandler m_ImpactTrigger;

        [Title("Logs")]

        [SerializeField, NotNull, PrefabObjectOnly]
        private Rigidbody m_LogPrefab;

        [SerializeField, Range(1, 100)]
        private int m_LogsCount = 6;

        [SerializeField, Range(0f, 100f)]
        private float m_LogsOffset = 2f;

        [Title("Effects")]

        [SerializeField]
        private GameObject m_TreeImpactFX;

        [SerializeField]
        private SoundPlayer m_TreeFallAudio;

        [SerializeField]
        private SoundPlayer m_TreeImpactAudio;

        [SerializeField]
        private ShakeSettings3D m_CameraShake = ShakeSettings3D.Default;

        private const float k_MinTimeToTriggerImpact = 1f;
        private const float k_MaxTimeToFall = 8f;
        private const float k_LogsForce = 50f;

        private bool m_IsFalling = false;
        private float m_TimeSinceFallStart;
        private Vector3 m_HitDirection = Vector3.zero;


        public override void DoHitEffects(DamageContext damageInfo)
        {
            m_TreeStump.SetActive(true);
            m_FallingTree.gameObject.SetActive(true);
        }

        /// <summary>
        /// Start tree fall
        /// </summary>
        /// <param name="dmgInfo"></param>
        public override void DoDestroyEffects(DamageContext dmgInfo)
        {
            m_HitDirection = dmgInfo.HitForce;
            StartTreeFall(m_HitDirection);
        }

        private void StartTreeFall(Vector3 hitForce) 
        {
            Vector3 force = new(hitForce.x, 0, hitForce.z);

            m_FallingTree.GetComponent<Collider>().enabled = true;
            m_FallingTree.isKinematic = false;
            m_FallingTree.AddForce(force, ForceMode.Impulse);

            m_ImpactTrigger.TriggerEnter += OnTreeImpact;

            m_TreeFallAudio.PlayAtPosition(transform.position, 1f, SelectionType.Random);
            m_TreeStump.SetActive(true);

            m_IsFalling = true;

            UpdateManager.AddUpdate(UpdateFall);
        }

        private void Awake() => ResetTree();

        private void OnDestroy()
        {
            if (m_IsFalling)
                UpdateManager.RemoveUpdate(UpdateFall);
        }

        private void ResetTree()
        {
            m_FallingTree.GetComponent<Collider>().enabled = false;
            m_FallingTree.isKinematic = true;
            m_FallingTree.gameObject.SetActive(false);

            m_TreeStump.SetActive(false);
            m_TimeSinceFallStart = 0f;
        }

        private void UpdateFall(float deltaTime)
        {
            // Force start the tree impact behaviour when the time limit is up or the velocity of the tree fall is close to 0.
            if (m_TimeSinceFallStart > k_MinTimeToTriggerImpact)
            {
                if (m_TimeSinceFallStart > k_MaxTimeToFall || m_FallingTree.angularVelocity.sqrMagnitude < 0.005f)
                    OnTreeImpact(null);
            }

            m_TimeSinceFallStart += deltaTime;
        }

        private void OnTreeImpact(Collider other)
        {
            m_IsFalling = false;

            UpdateManager.RemoveUpdate(UpdateFall);
            m_ImpactTrigger.TriggerEnter -= OnTreeImpact;

            StartCoroutine(C_DelayedImpact());
        }

        private IEnumerator C_DelayedImpact() 
        {
            yield return new WaitForSeconds(0.1f);

            m_FallingTree.GetComponent<Collider>().enabled = false;
            var fallingTree = m_FallingTree.transform;

            for (int i = 0; i < m_LogsCount; i++)
            {
                var log = Instantiate(m_LogPrefab, fallingTree);
                log.transform.SetParent(fallingTree, false);
                log.transform.localPosition = i * m_LogsOffset * Vector3.up;
                log.AddForce(k_LogsForce * Mathf.Sign(Random.Range(-100f, 100f)) * log.transform.right, ForceMode.Impulse);

                if (m_TreeImpactFX != null)
                    Instantiate(m_TreeImpactFX, log.position, log.rotation);

                log.transform.SetParent(null, true);
            }

            yield return null;
            m_FallingTree.GetComponent<Collider>().enabled = true;
            m_FallingTree.gameObject.SetActive(false);

            CameraShakeManagerBase.DoShake(transform.position, m_CameraShake, 1f);
            m_TreeImpactAudio.PlayAtPosition(transform.position, 1f, SelectionType.RandomExcludeLast);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_IsFalling = (bool)members[0];
            m_HitDirection = (Vector3)members[1];

            if (m_IsFalling)
            {
                StartTreeFall(m_HitDirection);
                print(m_IsFalling);
            }

            m_TreeStump.SetActive((bool)members[2]);
        }

        public object[] SaveMembers()
        {
            return new object[]
            {
                m_IsFalling,
                m_HitDirection,
                m_TreeStump.activeSelf
            };
        }
        #endregion
    }
}
