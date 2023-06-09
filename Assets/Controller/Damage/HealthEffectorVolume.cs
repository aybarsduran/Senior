using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class HealthEffectorVolume : MonoBehaviour
    {
        private enum StatInfluenceMode
        {
            IncreaseStat,
            DecreaseStat
        }

        public float RadiusMod { get; set; } = 1f;
        public float RadiationMod { get; set; } = 1f;

        [SerializeField]
        private SphereCollider m_InfluenceVolume;

        [SerializeField]
        private StatInfluenceMode m_InfluenceMode;


        [SerializeField, Range(0f, 100f)]
        private float m_Damage = 1f;

        [SerializeField, Range(0f, 100f)]
        private float m_Radius = 1f;

        private readonly List<ICharacter> m_CharactersInsideTrigger = new List<ICharacter>();
        private float m_TotalRadius;


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ICharacter character))
            {
                if (!m_CharactersInsideTrigger.Contains(character))
                    m_CharactersInsideTrigger.Add(character);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            CalculateRadius();

            for (int i = 0; i < m_CharactersInsideTrigger.Count; i++)
            {
                if (m_CharactersInsideTrigger[i].TryGetModule(out IHealthManager healthManager))
                {
                    float distanceToCharacter = Vector3.Distance(transform.position, other.transform.position);
                    float radFactor = 1f - distanceToCharacter / m_TotalRadius;
                    float healthChange = RadiationMod * m_Damage * radFactor * Time.deltaTime;

                    if (m_InfluenceMode == StatInfluenceMode.DecreaseStat)
                        healthManager.ReceiveDamage(healthChange);
                    else
                        healthManager.RestoreHealth(healthChange);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ICharacter character))
                m_CharactersInsideTrigger.Remove(character);
        }

        private void CalculateRadius()
        {
            m_TotalRadius = m_Radius * RadiusMod;
            m_InfluenceVolume.radius = m_TotalRadius;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var prevColor = Gizmos.color;
            Gizmos.color = new Color(1f, 1f, 1f, 0.3f) * Color.green;

            CalculateRadius();
            Gizmos.DrawSphere(transform.position, m_TotalRadius);

            Gizmos.color = prevColor;
        }

        private void OnValidate()
        {
            if (m_InfluenceVolume == null)
                m_InfluenceVolume = GetComponent<SphereCollider>();

            m_InfluenceVolume.isTrigger = true;
        }
#endif
    }
}