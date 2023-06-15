using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.WorldManagement
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class TemperatureEffector : MonoBehaviour
    {
        public float TemperatureStrength { get; set; }

        public float Radius
        {
            get => m_Radius;

            set
            {
                m_Radius = value;
                m_InfluenceVolume.radius = m_Radius;
            }
        }

        private float m_Radius;
        private Coroutine m_UpdateCoroutine;
        private SphereCollider m_InfluenceVolume;

        private readonly List<ICharacter> m_Characters = new List<ICharacter>();


        private void Awake()
        {
            m_InfluenceVolume = GetComponent<SphereCollider>();
            m_InfluenceVolume.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ICharacter character))
            {
                m_Characters.Add(character);

                if (m_UpdateCoroutine == null)
                    m_UpdateCoroutine = StartCoroutine(C_Update());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ICharacter character))
            {
                if (m_Characters.Remove(character) && m_Characters.Count == 0 && m_UpdateCoroutine != null)
                    StopCoroutine(m_UpdateCoroutine);
            }
        }

        private IEnumerator C_Update()
        {
            while (true)
            {
                foreach (var character in m_Characters)
                {
                    if (character.TryGetModule(out ITemperatureManager tempManager))
                    {
                        float distanceToChar = Vector3.Distance(transform.position, character.transform.position);
                        float tempFactor = 1f - distanceToChar / m_Radius;
                        float tempChange = TemperatureStrength * tempFactor * Time.deltaTime;

                        tempManager.Temperature = tempChange;
                    }
                }

                yield return null;
            }
        }
    }
}