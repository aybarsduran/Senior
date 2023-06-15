using UnityEngine;

namespace IdenticalStudios
{
    public sealed class ChildOfConstraint : MonoBehaviour
    {
        public enum UpdateMode
        {
            Automatic,
            Manual
        }

        public Transform Parent { get => m_Parent; set => m_Parent = value; }
        public UpdateMode UpdateType { get => m_UpdateMode; set => m_UpdateMode = value; }

        [SerializeField, InLineEditor]
        private Transform m_Parent;

        [SerializeField]
        private UpdateMode m_UpdateMode = UpdateMode.Automatic;

        [SpaceArea]

        [SerializeField]
        private bool m_CustomOffset;

        [SerializeField, ShowIf(nameof(m_CustomOffset), true)]
        private Vector3 m_PositionOffset;

        [SerializeField, ShowIf(nameof(m_CustomOffset), true)]
        private Vector3 m_RotationOffset;


        public void UpdateTransform()
        {
            if (m_Parent != null)
                transform.SetPositionAndRotation(m_Parent.position + m_Parent.TransformVector(m_PositionOffset), m_Parent.rotation * Quaternion.Euler(m_RotationOffset));
        }

        private void Awake() => SetParent(m_Parent);

        private void LateUpdate()
        {
            if (m_UpdateMode == UpdateMode.Automatic)
                UpdateTransform();
        }

        private void SetParent(Transform parent)
        {
            m_Parent = parent;
            if (m_Parent != null && !m_CustomOffset)
            {
                m_PositionOffset = m_Parent.InverseTransformPoint(transform.position);
                m_RotationOffset = (Quaternion.Inverse(m_Parent.rotation) * transform.rotation).eulerAngles;
            }
        }
    }
}