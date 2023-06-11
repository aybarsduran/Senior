using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public class StructureManager : MonoBehaviour, ISaveableComponent
    {
        #region Internal
        [Serializable]
        private struct BuildableData
        {
            public DataIdReference<BuildableDefinition> Buildable;
            public Vector3 Position;
            public Quaternion Rotation;
            public SocketState[] Sockets;
        }

        [Serializable]
        private struct SocketState
        {
            public DataIdReference<BuildableCategoryDefinition>[] OccupiedSpaces;

            public SocketState(DataIdReference<BuildableCategoryDefinition>[] occupiedSpaces)
            {
                OccupiedSpaces = occupiedSpaces;
            }
        }
        #endregion

        public List<StructureBuildable> Buildables => m_Buildables;

        [SerializeField]
        private SoundPlayer m_BuildAudio;

        private readonly List<StructureBuildable> m_Buildables = new();


		public bool HasCollider(Collider col)
		{
            for (int i = 0; i < m_Buildables.Count; i++)
            {
                if (m_Buildables[i].HasCollider(col))
                    return true;
            }

			return false;
		}

        public void PlayBuildEffects() => m_BuildAudio.Play2D();

        public void AddPart(StructureBuildable buildable)
		{
			if (!m_Buildables.Contains(buildable))
			{
				m_Buildables.Add(buildable);

                buildable.ParentStructure = this;
                buildable.transform.SetParent(transform);

                OccupySurroundingSockets(buildable);
            }
		}

        public void RemovePart(StructureBuildable buildable)
        {
            if (m_Buildables.Remove(buildable))
            {
                UnoccupySurroundingSockets(buildable);
                Destroy(buildable.gameObject);
            }
        }

        private void OccupySurroundingSockets(StructureBuildable buildable)
        {
            Collider[] overlappingStuff = Physics.OverlapBox(buildable.Bounds.center, buildable.Bounds.extents, buildable.transform.rotation, BuildingManager.FreePlacementMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < overlappingStuff.Length; i++)
            {
                if (!overlappingStuff[i].isTrigger)
                    continue;

                Socket socket = overlappingStuff[i].GetComponent<Socket>();

                if (socket != null && socket.SupportsBuildable(buildable) && !socket.HasSpaceForBuildable(BuildingManager.FreePlacementMask, buildable))
                    socket.OccupySpaces(buildable);
            }
        }

        private void UnoccupySurroundingSockets(StructureBuildable buildable)
        {
            if (buildable.OccupiedSocketPosition == Vector3.zero)
                return;

            LayerMask buildableMask = (1 << BuildingManager.BuildableLayer) | (1 << BuildingManager.BuildablePreviewLayer);
            Collider[] overlappingStuff = Physics.OverlapBox(buildable.Bounds.center, buildable.Bounds.extents, buildable.transform.rotation, buildableMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < overlappingStuff.Length; i++)
            {
                if (!overlappingStuff[i].isTrigger)
                    continue;

                if (overlappingStuff[i].TryGetComponent<Socket>(out var socket))
                    socket.UnoccupySpaces(buildable);
            }
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            var buildableData = members[0] as BuildableData[];

            // Load buildables into structure.
            foreach (BuildableData data in buildableData)
            {
                StructureBuildable buildablePrefab = BuildableDefinition.GetWithId(data.Buildable).Prefab as StructureBuildable;
                StructureBuildable buildable = null;

                if (buildablePrefab != null)
                {
                    buildable = Instantiate(buildablePrefab, data.Position, data.Rotation, transform);
                    buildable.ParentStructure = this;

                    for (int i = 0; i < buildable.Sockets.Length; i++)
                        buildable.Sockets[i].OccupiedSpaces = data.Sockets[i].OccupiedSpaces;
                }

                if (!m_Buildables.Contains(buildable))
                    m_Buildables.Add(buildable);
            }      
        }

        public object[] SaveMembers()
        {
            BuildableData[] buildableStates = new BuildableData[m_Buildables.Count];

            // Save buildables data.
            for (int i = 0; i < m_Buildables.Count; i++)
            {
                var buildable = m_Buildables[i];
                var buildableSockets = m_Buildables[i].Sockets;

                BuildableData state = new()
                {
                    Buildable = buildable.Definition.Id,
                    Position = buildable.transform.position,
                    Rotation = buildable.transform.rotation,
                    Sockets = new SocketState[buildableSockets.Length],
                };

                for (int j = 0; j < buildableSockets.Length; j++)
                    state.Sockets[j] = new SocketState(buildableSockets[j].OccupiedSpaces);

                buildableStates[i] = state;
            }

            object[] members = new object[]
            {
                buildableStates,
            };

            return members;
        }
        #endregion
    }
}
