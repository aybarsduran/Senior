using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [System.Serializable]
    public abstract class PlacementState
    {
        public abstract Buildable Buildable { get; }
        public abstract bool ContinueBuildingOnPlaced { get; }
        protected ICharacter Character { get; private set; }

        [SerializeField]
        [Tooltip("Should the placeables follow the rotation of the character?")]
        protected bool m_FollowCharacterRotation;

        [SerializeField, Range(0f, 70f)]
        [Tooltip("Max angle for detecting nearby sockets.")]
        protected float m_ViewAngleThreshold = 35f;

        [SerializeField, Range(0f, 360)]
        [Tooltip("How fast can the placeables be rotated.")]
        protected float m_RotationSpeed = 45f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("Max building range.")]
        protected float m_BuildRange = 4f;


        public virtual void Initialize(ICharacter character) => Character = character;

        /// <returns> True if enabled. </returns>
        public abstract bool TrySetBuildable(Buildable buildable);

        /// <returns> True if placement allowed. </returns>
        public abstract void UpdatePlacement(float rotationOffset);

        /// <summary>
        /// Place active buildable.
        /// </summary>
        public abstract bool TryPlaceActiveBuildable();

        /// <summary>
        /// Selects the next/previous buildable.
        /// </summary>
        public virtual BuildableDefinition SelectNextBuildable(bool next) => Buildable != null ? Buildable.Definition : null;

        protected void DoFreePlacement(Buildable buildable, Quaternion rotation, out Collider surface)
        {
            Vector3 targetPosition;
            Quaternion targetRotation = !m_FollowCharacterRotation ? rotation : Character.transform.rotation * rotation;

            LayerMask freePlacementMask = BuildingManager.FreePlacementMask;
            Ray ray = UnityUtils.CachedMainCamera.ViewportPointToRay(Vector3.one * 0.5f);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, m_BuildRange, freePlacementMask, QueryTriggerInteraction.Ignore))
            {
                targetPosition = hitInfo.point;
                surface = hitInfo.collider;
            }
            else
            {
                Vector3 currentPos = Character.transform.position + Character.transform.forward * m_BuildRange;
                Vector3 startPos = buildable.transform.position + new Vector3(0, 0.25f, 0);

                if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 1f, freePlacementMask, QueryTriggerInteraction.Ignore))
                {
                    currentPos.y = hit.point.y;
                    surface = hit.collider;
                }
                else
                    surface = null;

                targetPosition = currentPos;
            }

            buildable.transform.SetPositionAndRotation(targetPosition, targetRotation);
        }
    }
}
