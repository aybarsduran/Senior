using UnityEngine;

namespace IdenticalStudios
{
    [ExecuteAlways]
    public class MoveToSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_Offset;


        private void Start()
        {
            if (!Application.isEditor)
                Destroy(this);
        }

#if UNITY_EDITOR
        public void MoveToRandomPoint()
        {
            UnityEditor.Undo.RecordObject(transform, "MoveSpawnPoint");

            // Search for random spawn point.
            var spawnPoint = SpawnPointData.Default;
            var foundSpawnPoints = FindObjectsOfType<SpawnPoint>();

            if (foundSpawnPoints != null && foundSpawnPoints.Length > 0)
                spawnPoint = foundSpawnPoints.SelectRandom().GetSpawnPoint();

            transform.SetPositionAndRotation(spawnPoint.Position + m_Offset, spawnPoint.Rotation);
        }
#endif
    }
}