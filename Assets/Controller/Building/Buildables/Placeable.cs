using System;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public abstract class Placeable : MonoBehaviour
    {
        public Bounds Bounds => GetWorldBounds();
        public Bounds LocalBounds => m_LocalBounds;
        public Collider[] Colliders => m_Colliders;

        public bool PlaceOnPlaceables => m_PlaceOnPlaceables;

        [Title("Settings (Placeable)")]

        [SerializeField]
        private bool m_PlaceOnPlaceables = true;

        [SerializeField, Range(0f, 1f)]
        private float m_BoundsGroundOffset = 0f;

        [SerializeField, Range(0.1f, 2f)]
        private float m_BoundsScale = 1f;

        [SerializeField, HideInInspector]
        private Bounds m_RawLocalBounds;

        [SerializeField, HideInInspector]
        private Bounds m_LocalBounds;

        [SpaceArea]

        [SerializeField]
        private SoundPlayer m_PlacementAudio;

        [SerializeField]
        private GameObject m_PlacementFX;

        private Collider[] m_Colliders;


        public void CalculateLocalBounds()
        {
            // Calculate the bounds without modifications
            m_RawLocalBounds = new Bounds(transform.position, Vector3.zero);
            var renderers = GetComponentsInChildren<MeshRenderer>();

            Quaternion initialRotation = transform.rotation;
            transform.rotation = Quaternion.identity;

            for(int i = 0;i < renderers.Length;i++)
                m_RawLocalBounds.Encapsulate(renderers[i].bounds);

            m_RawLocalBounds = new Bounds(m_RawLocalBounds.center - transform.position, m_RawLocalBounds.size);

            transform.rotation = initialRotation;

            // Calculate the bounds with modifications
            Vector3 center = m_RawLocalBounds.center;
            Vector3 extents = m_RawLocalBounds.extents;

            float upExtent = extents.y;

            Vector3 offset = m_BoundsGroundOffset * upExtent * Vector3.up;

            center += offset;
            extents -= offset;
            extents = Vector3.Scale(extents, new Vector3(m_BoundsScale, 1f, m_BoundsScale));

            m_LocalBounds = new Bounds(center, extents * 2);
        }

        public bool HasCollider(Collider col)
        {
            for (int i = 0;i < m_Colliders.Length;i++)
            {
                if (m_Colliders[i] == col)
                    return true;
            }

            return false;
        }

        protected virtual void Awake() => m_Colliders = GetComponentsInChildren<Collider>(true);

        protected void DoPlacementEffects()
        {
            m_PlacementAudio.Play2D();

            if (m_PlacementFX != null)
                Instantiate(m_PlacementFX, transform.position, Quaternion.identity, null);
        }

        protected void EnableColliders(bool enable)
        {
            for (int i = 0; i < m_Colliders.Length; i++)
                m_Colliders[i].enabled = enable;
        }

        private Bounds GetWorldBounds()
        {
            Bounds worldBounds = m_LocalBounds;
            worldBounds.center = transform.position + transform.TransformVector(worldBounds.center);

            return worldBounds;
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            if (m_RawLocalBounds.size == Vector3.zero)
                CalculateLocalBounds();
        }
#endif
    }
}