using IdenticalStudios.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IdenticalStudios.Demo
{
    [RequireComponent(typeof(Collider))]
    public class ItemSpawner : MonoBehaviour
    {
        [Title("General")]

        [SerializeField, ReorderableList(HasLabels = false)]
        private ItemGenerator[] m_ItemsToSpawn;

        [SpaceArea]

        [SerializeField]
        private Vector2Int m_ItemSpawnCount;

        [SerializeField, Range(0f, 10f)]
        private float m_SpawnDelay = 0.5f;

        [SerializeField, Range(0f, 10f)]
        private float m_ConsecutiveSpawnDelay = 0.1f;

        [SerializeField, Range(0f, 100f)]
        private float m_ItemDestroyDelay = 15f;

        [SpaceArea]

        [SerializeField]
        private Vector3 m_RandomRotation = Vector3.zero;

        [SerializeField, Range(0f, 100f)]
        private float m_PositionForce = 0f;

        [SerializeField, Range(0f, 100f)]
        private float m_AngularForce = 0f;

        [Title("Effects")]

        [SerializeField]
        private ParticleSystem m_ParticleEffects = null;

        [SerializeField]
        private SoundPlayer m_StartSpawnAudio = null;

        [SerializeField]
        private SoundPlayer m_EndSpawnAudio = null;

        private BoxCollider m_Collider;
        private WaitForSeconds m_TimeBetweenSpawns;


        public void SpawnItems()
        {
            float spawnCount = Mathf.Clamp(m_ItemSpawnCount.GetRandomInt(), 0, m_ItemsToSpawn.Length);

            List<GameObject> itemsToSpawn = new List<GameObject>();

            for (int i = 0; i < spawnCount; i++)
            {
                var itemToSpawn = m_ItemsToSpawn.SelectRandom();    
                ItemDefinition itemDef = itemToSpawn.GetItemDefinition();

                if (itemDef != null && itemDef.Pickup != null)
                    itemsToSpawn.Add(itemDef.Pickup.gameObject);
                else
                    spawnCount--;
            }

            StartCoroutine(C_SpawnItems(itemsToSpawn));
        }

        private void Start()
        {
            m_Collider = GetComponent<BoxCollider>();
            m_TimeBetweenSpawns = new WaitForSeconds(m_ConsecutiveSpawnDelay);
        }

        private IEnumerator C_SpawnItems(List<GameObject> itemsToSpawn)
        {
            yield return new WaitForSeconds(m_SpawnDelay);

            m_StartSpawnAudio.PlayAtPosition(transform.position);

            for (int i = 0; i < itemsToSpawn.Count; i++)
            {
                Quaternion spawnRotation = Quaternion.Euler(
                    Random.Range(-Mathf.Abs(m_RandomRotation.x), Mathf.Abs(m_RandomRotation.x)),
                    Random.Range(-Mathf.Abs(m_RandomRotation.y), Mathf.Abs(m_RandomRotation.y)),
                    Random.Range(-Mathf.Abs(m_RandomRotation.z), Mathf.Abs(m_RandomRotation.z))
                );

                GameObject pickup = Instantiate(itemsToSpawn[i], m_Collider.bounds.GetRandomPoint(), spawnRotation);

                if (m_ParticleEffects != null)
                    Instantiate(m_ParticleEffects, pickup.transform.position, spawnRotation);

                if (m_PositionForce > 0.01f && pickup.TryGetComponent(out Rigidbody rigidB))
                {
                    rigidB.velocity = Random.insideUnitSphere.normalized * m_PositionForce;
                    rigidB.angularVelocity = spawnRotation.eulerAngles * m_AngularForce; ;
                }

                if (pickup != null && m_ItemDestroyDelay > 0.01f)
                    Destroy(pickup, m_ItemDestroyDelay);

                yield return m_TimeBetweenSpawns;
            }

            m_EndSpawnAudio.PlayAtPosition(transform.position);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                m_TimeBetweenSpawns = new WaitForSeconds(m_ConsecutiveSpawnDelay);
        }
#endif
    }
}