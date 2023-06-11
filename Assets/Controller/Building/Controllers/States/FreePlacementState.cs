using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [System.Serializable]
    public class FreePlacementState : PlacementState
    {
        public override Buildable Buildable => m_Buildable;
        public override bool ContinueBuildingOnPlaced => false;

        private bool m_PlacementAllowed;
        private FreeBuildable m_Buildable;
        private Quaternion m_BuildablePrefabRotation;

        private Collider m_Surface;
        private readonly Collider[] m_Results = new Collider[10];


        public override bool TrySetBuildable(Buildable buildable)
        {
            if (buildable == null)
            {
                m_Buildable = null;
                return false;
            }

            m_Buildable = buildable as FreeBuildable;
            m_BuildablePrefabRotation = buildable.Definition.Prefab.transform.rotation;

            if (m_Buildable != null)
            {
                m_PlacementAllowed = true;
                m_Buildable.MaterialEffect.EnableCustomEffect(BuildingManager.PlacementAllowedMaterialEffect);
                return true;   
            }

            return false;
        }

        public override void UpdatePlacement(float rotationOffset)
        {
            bool isPlacementAllowed = !CheckForCollisions(m_Buildable);

            // Update the materials...
            if (isPlacementAllowed != m_PlacementAllowed)
                m_Buildable.MaterialEffect.EnableCustomEffect(isPlacementAllowed ? BuildingManager.PlacementAllowedMaterialEffect : BuildingManager.PlacementDeniedMaterialEffect);

            m_PlacementAllowed = isPlacementAllowed;

            DoFreePlacement(m_Buildable, Quaternion.Euler(0f, rotationOffset * m_RotationSpeed, 0f) * m_BuildablePrefabRotation, out m_Surface);
        }

        public override bool TryPlaceActiveBuildable()
        {
            if (!m_PlacementAllowed)
                return false;

            var preview = m_Buildable.gameObject.AddComponent<FreeBuildablePreview>();
            preview.EnablePreview();
            preview.AddBuildable(m_Buildable);

            m_Buildable.OnPlaced();

            return true;
        }

        private bool CheckForCollisions(FreeBuildable buildable)
        {
            bool canPlace = buildable.PlaceOnPlaceables || m_Surface == null || m_Surface.GetComponent<Buildable>() == null;

            if (!canPlace)
                return false;

            var bounds = buildable.Bounds;
            int size = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, m_Results, buildable.transform.rotation, BuildingManager.OverlapCheckMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < size; i++)
            {
                var collider = m_Results[i];

                if (!buildable.HasCollider(collider) && collider is not TerrainCollider)
                    return true;
            }

            return false;
        }
    }
}
