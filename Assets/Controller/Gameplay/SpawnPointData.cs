using UnityEngine;

namespace IdenticalStudios
{
    public struct SpawnPointData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public static SpawnPointData Default => defaultSpawnPoint;

        private static readonly SpawnPointData defaultSpawnPoint = new SpawnPointData(Vector3.zero, Quaternion.identity);


        public SpawnPointData(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        public bool IsDefault() => this == Default;

        public static bool operator ==(SpawnPointData thisSpawnPoint, SpawnPointData spawnPoint)
        {
            bool isEqual = (thisSpawnPoint.Position - spawnPoint.Position).sqrMagnitude < 0.1f;
            isEqual &= Quaternion.Angle(thisSpawnPoint.Rotation, spawnPoint.Rotation) < 0.1f;

            return isEqual;
        }

        public static bool operator !=(SpawnPointData thisSpawnPoint, SpawnPointData spawnPoint)
        {
            bool isNotEqual = (thisSpawnPoint.Position - spawnPoint.Position).sqrMagnitude > 0.1f;
            isNotEqual |= Quaternion.Angle(thisSpawnPoint.Rotation, spawnPoint.Rotation) > 0.1f;

            return isNotEqual;
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}