using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [System.Serializable]
    public class StructurePlacementState : PlacementState
    {
        public override Buildable Buildable => m_Buildable;
        public override bool ContinueBuildingOnPlaced => true;

        private bool m_PlacementAllowed;
        private StructureBuildable m_Buildable;
        private Quaternion m_BuildablePrefabRotation;
        private Socket m_Socket;
        private Collider m_Surface;
        
        
        public override bool TrySetBuildable(Buildable buildable)
        {
            if (buildable == null)
            {
                m_Buildable = null;
                return false;
            }

            m_Buildable = buildable as StructureBuildable;
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
            m_Socket = FindSocket(m_Buildable);

            if (m_Buildable.RequiresSockets && m_Socket == null)
                isPlacementAllowed = false;

            // Update the materials...
            if (isPlacementAllowed != m_PlacementAllowed)
                m_Buildable.MaterialEffect.EnableCustomEffect(isPlacementAllowed ? BuildingManager.PlacementAllowedMaterialEffect : BuildingManager.PlacementDeniedMaterialEffect);

            m_PlacementAllowed = isPlacementAllowed;

            if (m_Socket != null)
                SnapToSocket(m_Buildable, m_Socket);
            else
                DoFreePlacement(m_Buildable, Quaternion.Euler(0f, rotationOffset * m_RotationSpeed, 0f) * m_BuildablePrefabRotation, out m_Surface);
        }

        public override bool TryPlaceActiveBuildable()
        {
            if (!m_PlacementAllowed)
                return false;

            var structure = m_Socket != null ?
                m_Socket.ParentBuildable.ParentStructure :
                Object.Instantiate(BuildingManager.StructurePrefab, m_Buildable.transform.position, m_Buildable.transform.rotation);

            StructurePreview preview = structure.gameObject.GetOrAddComponent<StructurePreview>();
            preview.EnablePreview();
            preview.AddBuildable(m_Buildable);

            m_Buildable.OnPlaced();
            structure.AddPart(m_Buildable);

            return true;
        }

        public override BuildableDefinition SelectNextBuildable(bool next)
        {
            var buildableDefs = BuildingManager.CustomBuildingDefinitions;

            int customBuildableIdx = buildableDefs.GetIndexOfDefinition(m_Buildable.Definition);

            if (customBuildableIdx == -1)
                customBuildableIdx = 0;

            customBuildableIdx = (int)Mathf.Repeat(customBuildableIdx + (next ? 1 : -1), buildableDefs.Length);

            return buildableDefs[customBuildableIdx];
        }

        private Socket FindSocket(StructureBuildable buildable)
        {
            LayerMask buildablesMask = (1 << BuildingManager.BuildableLayer) | (1 << BuildingManager.BuildablePreviewLayer);
            int overlapCount = PhysicsUtils.OverlapSphereNonAlloc(Character.transform.position, m_BuildRange, out var colliders, buildablesMask);

            float smallestAngleToSocket = Mathf.Infinity;
            Socket socket = null;

            // Loop through all buildables in proximity and calculate which socket is the closest in terms of distance & angle
            for (int i = 0; i < overlapCount; i++)
            {
                StructureBuildable proximityBuildable = colliders[i].GetComponent<StructureBuildable>();

                // Skip the checks on this object if there's no buildable component or if the buildable doesn't have sockets
                if (proximityBuildable == null || proximityBuildable.Sockets.Length == 0)
                    continue;

                // Loop through all sockets, compare it to the last one that was checked, see if we find a better one to snap to
                foreach (var sckt in proximityBuildable.Sockets)
                {
                    var viewRay = new Ray(Character.ViewTransform.position, Character.ViewTransform.forward);
                    CheckSocket(viewRay, buildable, sckt, ref socket, ref smallestAngleToSocket);
                }
            }

            return socket;
        }

        private void SnapToSocket(Buildable buildable, Socket socket)
        {
            var buildableOffset = socket.GetBuildableOffset(buildable.Definition.ParentGroup.Id);
            var parentBuildableTransform = socket.ParentBuildable.transform;
            
            var position = socket.transform.position + parentBuildableTransform.TransformVector(buildableOffset.PositionOffset);
            var rotation = parentBuildableTransform.rotation * buildableOffset.RotationOffset;
            
            buildable.transform.SetPositionAndRotation(position, rotation);
        }

        private void CheckSocket(Ray viewRay, StructureBuildable buildable, Socket socket, ref Socket bestMatchSocket, ref float bestMatchAngle)
        {
            if (!socket.SupportsBuildable(buildable))
                return;

            if ((socket.transform.position - Character.transform.position).sqrMagnitude > m_BuildRange * m_BuildRange)
                return;

            float angleToSocket = Vector3.Angle(viewRay.direction, socket.transform.position - viewRay.origin);

            if (angleToSocket < bestMatchAngle && angleToSocket < m_ViewAngleThreshold)
            {
                bestMatchAngle = angleToSocket;
                bestMatchSocket = socket;
            }
        }

        private bool CheckForCollisions(StructureBuildable buildable)
        {
            bool canPlace = buildable.PlaceOnPlaceables || m_Surface == null || m_Surface.GetComponent<Buildable>() == null;

            if (!canPlace)
                return false;

            Bounds bounds = buildable.Bounds;
            LayerMask overlapMask = BuildingManager.OverlapCheckMask;

            int overlapCount = PhysicsUtils.OverlapBoxNonAlloc(bounds.center, bounds.extents, buildable.transform.rotation, out var overlappingColliders, overlapMask);

            for (int i = 0; i < overlapCount; i++)
            {
                var collider = overlappingColliders[i];

                if ((collider as TerrainCollider) == null)
                {
                    if (m_Socket != null && collider.TryGetComponent<StructureBuildable>(out var structureBuildable))
                    {
                        bool isSameStructure = structureBuildable.ParentStructure == m_Socket.ParentBuildable.ParentStructure;

                        if (!isSameStructure)
                            return true;
                    }
                    else
                        return true;
                }
            }

            return false;
        }
    }
}