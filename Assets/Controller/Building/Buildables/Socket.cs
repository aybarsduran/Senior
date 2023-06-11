using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.BuildingSystem
{
    public class Socket : MonoBehaviour
    {
        #region Internal
        [Serializable]
        public class BuildableOffset
        {
            public DataIdReference<BuildableCategoryDefinition> Category => m_Category;

            public Vector3 PositionOffset => m_PositionOffset;
            public Vector3 RotationOffsetEuler => m_RotationOffset;
            public Quaternion RotationOffset => Quaternion.Euler(m_RotationOffset);

            [SerializeField, DataReferenceDetails(HasLabel = false, HasNullElement = false)]
            private DataIdReference<BuildableCategoryDefinition> m_Category;

            [SerializeField]
            private Vector3 m_PositionOffset;

            [SerializeField]
            private Vector3 m_RotationOffset;


#if UNITY_EDITOR
            public void SetBuildable(DataIdReference<BuildableCategoryDefinition> category) => m_Category = category;
            public void SetPositionOffset(Vector3 offset) => m_PositionOffset = offset;
            public void SetRotationOffset(Vector3 offset) => m_RotationOffset = offset;
#endif
        }
        #endregion

        public DataIdReference<BuildableCategoryDefinition>[] OccupiedSpaces 
        {
            get => m_OccupiedSpaces.ToArray();
            set
            {
                m_OccupiedSpaces.Clear();
                m_OccupiedSpaces.AddRange(value);
            }
        }

        public StructureBuildable ParentBuildable
        {
            get
            {
                if (m_ParentBuildable == null)
                    m_ParentBuildable = GetComponentInParent<StructureBuildable>();

                return m_ParentBuildable;
            }
        }

        public BuildableOffset[] Offsets => m_Offsets;

        [SerializeField]
		private BuildableOffset[] m_Offsets;

		[SerializeField]
		private float m_Radius = 1f;

        private StructureBuildable m_ParentBuildable;
        private readonly List<DataIdReference<BuildableCategoryDefinition>> m_OccupiedSpaces = new();


        private void Awake()
        {
            var sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = m_Radius;
        }

        public BuildableOffset GetBuildableOffset(int buildableId)
        {
            BuildableOffset offset = null;

            for (int i = 0;i < m_Offsets.Length;i++)
            {
                if (m_Offsets[i].Category == buildableId)
                {
                    offset = m_Offsets[i];
                    break;
                }
            }

            return offset;
        }
			
		public bool HasSpaceForBuildable(LayerMask mask, StructureBuildable buildable)
		{
            // Get the objects that overlap this socket.
            var overlappingStuff = Physics.OverlapSphere(transform.position, m_Radius, mask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < overlappingStuff.Length; i++)
            {
                if (!ParentBuildable.HasCollider(overlappingStuff[i]))
                    return false;

                if (m_ParentBuildable != buildable)
                {
                    if (!m_ParentBuildable.ParentStructure.HasCollider(overlappingStuff[i]) && overlappingStuff[i] as TerrainCollider == null)
                        return false;
                }
                else
                {
                    if (!m_ParentBuildable.HasCollider(overlappingStuff[i]) && overlappingStuff[i] as TerrainCollider == null)
                        return false;
                }
            }

            return true;
        }

		public bool OccupySpaces(StructureBuildable buildable)
		{
            if (!m_OccupiedSpaces.ContainsAnyDef(buildable.SpacesToOccupy))
            {
                buildable.OccupiedSocketPosition = transform.position;
                m_OccupiedSpaces.AddRange(buildable.SpacesToOccupy);

                return true;
            }

            return false;
		}

        public bool UnoccupySpaces(StructureBuildable buildable)
        {
            if (m_OccupiedSpaces.ContainsAnyDef(buildable.SpacesToOccupy))
            {
                buildable.OccupiedSocketPosition = Vector3.zero;

                for (int i = 0; i < buildable.SpacesToOccupy.Length; i++)
                    m_OccupiedSpaces.Remove(buildable.SpacesToOccupy[i]);

                return true;
            }

            return false;
        }

        public bool SupportsBuildable(StructureBuildable buildable)
		{
            int categoryId = buildable.Definition.ParentGroup.Id;

            for (int i = 0; i < m_Offsets.Length; i++)
			{
				if (m_Offsets[i].Category == categoryId && !m_OccupiedSpaces.Contains(buildable.RequiredSpace))
					return true;
			}

			return false;
		}

        #region Editor
#if UNITY_EDITOR
        private void OnDrawGizmos()
		{
			var oldMatrix = Gizmos.matrix;

			Gizmos.color = new Color(.1f, 1f, .1f, 0.65f);
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * 0.2f);

			Gizmos.DrawCube(Vector3.zero, Vector3.one);

			Gizmos.matrix = oldMatrix;
		}

		private void OnDrawGizmosSelected()
		{
            Handles.color = new Color(0.3f, 0.4f, 0.3f, 0.2f);
            Handles.SphereHandleCap(24, transform.position, Quaternion.identity, m_Radius * 2f, EventType.Repaint);
		}
#endif
        #endregion
    }
}