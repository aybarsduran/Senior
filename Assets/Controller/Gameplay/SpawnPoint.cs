using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    public class SpawnPoint : MonoBehaviour
    {
        public static SpawnPoint[] SpawnPoints => s_SpawnPoints.ToArray();

        private readonly static List<SpawnPoint> s_SpawnPoints = new();


        public virtual SpawnPointData GetSpawnPoint()
        {
            float yAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, yAngle, 0f);

            return new SpawnPointData(transform.position, rotation);
        }

        private void OnEnable() => s_SpawnPoints.Add(this);
        private void OnDisable() => s_SpawnPoints.Remove(this);

#if UNITY_EDITOR
        public void SnapToGround() 
        {
            // Snaps the spawn point position to the ground.
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 10f))
                transform.position = hitInfo.point + Vector3.up * 0.05f;
            else if (Physics.Raycast(transform.position + Vector3.up * 3f, Vector3.down, out hitInfo, 10f))
                transform.position = hitInfo.point + Vector3.up * 0.05f;
        }

        private void OnDrawGizmosSelected()
        {
            var prevColor = Gizmos.color;
            Gizmos.color = new Color(0.1f, 0.9f, 0.1f, 0.35f);

            const float gizmoWidth = 0.5f;
            const float gizmoHeight = 1.8f;

            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + gizmoHeight / 2, transform.position.z), new Vector3(gizmoWidth, gizmoHeight, gizmoWidth));

            Gizmos.color = prevColor;
        }
#endif
    }
}