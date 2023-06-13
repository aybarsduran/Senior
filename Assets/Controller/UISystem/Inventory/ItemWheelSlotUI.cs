using UnityEngine;

namespace IdenticalStudios.UISystem
{
    /// <summary>
    /// TODO: Remove and calculate the angle directly from the item wheel.
    /// </summary>
    public sealed class ItemWheelSlotUI : ItemSlotUI
    {
        public Vector2 AngleCoverage => new(m_AngleStart, m_AngleEnd);

        [Title("Item Wheel Slot")]

        [SerializeField, Range(-360f, 360f)]
        private float m_AngleStart;

        [SerializeField, Range(-360f, 360f)]
        private float m_AngleEnd;


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (m_AngleStart < 0 || m_AngleEnd < 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.blue;

            Gizmos.DrawRay(transform.parent.position, Quaternion.Euler(0, 0, m_AngleStart) * Vector3.up * 150);
            Gizmos.DrawRay(transform.parent.position, Quaternion.Euler(0, 0, m_AngleEnd) * Vector3.up * 150);
        }
#endif
    }
}