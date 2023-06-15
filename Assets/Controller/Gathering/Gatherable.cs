using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.ResourceGathering
{
    /// <summary>
    /// TODO: Implement respawning.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Gatherable : MonoBehaviour, IGatherable, ISaveableComponent
    {
        public float Health => m_Health;
        public float MaxHealth => m_MaxHealth;
        public float GatherRadius => m_GatherRadius;
        public Vector3 GatherOffset => m_GatherOffset;
        public GatherableDefinition Definition => m_Definition;

        [SerializeField]
        private GatherableDefinition m_Definition;

        [SpaceArea]

        [SerializeField]
        [Help("Visuals to DISABLE when a character interacts with this gatherable.", UnityMessageType.None)]
        private GameObject m_BaseVisuals;

        [SpaceArea]

        [SerializeField]
        [Help("Collider to DISABLE when a character interacts with this gatherable.", UnityMessageType.None)]
        private Collider m_Collider;

        [Title("Gathering")]

        [SerializeField, Range(0f, 1000f)]
        [NewLabel("Health")]
        private float m_MaxHealth = 100f;

        [SerializeField]
        private Vector3 m_GatherOffset;

        [SerializeField, Range(0.1f, 1f)]
        private float m_GatherRadius = 0.35f;

        [SerializeField, Disable]
        private float m_Health;

        private GatherableBehaviour[] m_Behaviours;
        private static readonly Dictionary<GatherableDefinition, List<Gatherable>> s_AllGatherables = new();


        #region Static Methods
        public static bool TryGetAllGatherablesWithDefinition(GatherableDefinition def, out List<Gatherable> gatherables)
        {
            return s_AllGatherables.TryGetValue(def, out gatherables);
        }

        private static void RegisterGatherable(Gatherable gatherable)
        {
            if (gatherable.Definition == null)
            {
                Debug.LogError("Unassigned gatherable definition", gatherable.gameObject);
                return;
            }

            if (s_AllGatherables.TryGetValue(gatherable.Definition, out List<Gatherable> gatherables))
                gatherables.Add(gatherable);
            else
            {
                List<Gatherable> gatherableList = new() { gatherable };
                s_AllGatherables.Add(gatherable.Definition, gatherableList);
            }
        }

        private static void UnregisterGatherable(Gatherable gatherable)
        {
            if (s_AllGatherables.TryGetValue(gatherable.Definition, out List<Gatherable> gatherableList))
                gatherableList.Remove(gatherable);
        }
        #endregion

        public DamageResult HandleDamage(float damage, DamageContext dmgContext)
        {
            damage = Mathf.Abs(damage);

            if (m_Health < 0.001f || damage < 0.01f)
                return DamageResult.Ignored;

            m_BaseVisuals.SetActive(false);
            m_Health = Mathf.Clamp(m_Health - damage, 0f, m_MaxHealth);

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].DoHitEffects(dmgContext);

            if (m_Health < 0.1f)
            {
                m_Collider.enabled = false;

                for (int i = 0; i < m_Behaviours.Length; i++)
                    m_Behaviours[i].DoDestroyEffects(dmgContext);

                return DamageResult.Critical;
            }

            return DamageResult.Default;
        }

        protected void ResetHealth()
        {
            m_Health = m_MaxHealth;
            m_Collider.enabled = true;
            m_BaseVisuals.SetActive(true);
        }

        private void Awake()
        {
            m_Behaviours = GetComponents<GatherableBehaviour>();
            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].InitializeBehaviour(this);

            ResetHealth();
        }

        private void OnEnable() => RegisterGatherable(this);
        private void OnDisable() => UnregisterGatherable(this);

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            float health = (float)members[0];

            if (Mathf.Abs(m_MaxHealth - health) > 0.1f)
            {
                if (health > 0.1f)
                    HandleDamage(m_MaxHealth - health, DamageContext.Default);
                else
                {
                    m_BaseVisuals.SetActive(false);
                    m_Collider.enabled = false;
                    m_Health = 0f;
                }
            }
        }

        public object[] SaveMembers()
        {
            return new object[]
            {
                m_Health,
            };
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Collider == null)
                m_Collider = GetComponent<Collider>();
        }

        private void OnDrawGizmosSelected()
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 gatherPoint = transform.position + transform.TransformVector(m_GatherOffset);

                UnityEditor.Handles.CircleHandleCap(0, gatherPoint, Quaternion.LookRotation(Vector3.up), m_GatherRadius, EventType.Repaint);

                UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.5f);
                UnityEditor.Handles.SphereHandleCap(0, gatherPoint, Quaternion.identity, 0.1f, EventType.Repaint);
                UnityEditor.Handles.color = Color.white;

                UnityEditor.Handles.Label(gatherPoint, "Gather Position");
            }
        }
#endif
    }
}