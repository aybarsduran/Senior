using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class FPFirearmPartsCorrector : MonoBehaviour
	{
        #region Internal
        [Serializable]
        public class MovingPart
        {
            public Transform Transform;
            public Vector3 PositionOffset;
            public Vector3 RotationOffset;
        }
        #endregion

        [SerializeField, Range(0f, 10f)]
        private float m_MoveDelay = 0.1f;


        [SerializeField, ReorderableList(childLabel: "Part")]
        private MovingPart[] m_MovingParts;

        private IFirearm m_Firearm;

        private float m_CanUpdateMovingPartsTime;
        private bool m_UpdateMovingParts;


        private void Awake() => m_Firearm = GetComponentInParent<IFirearm>();

        private void LateUpdate()
        {
            // If this weapon's magazine is empty, update the moving parts
            if (m_Firearm.Reloader != null && m_Firearm.Reloader.IsMagazineEmpty)
            {
                if (m_Firearm.Reloader.IsReloading)
                {
                    m_UpdateMovingParts = false;
                    return;
                }

                if (!m_UpdateMovingParts)
                {
                    m_UpdateMovingParts = true;
                    m_CanUpdateMovingPartsTime = Time.time + m_MoveDelay;
                }

                if (m_CanUpdateMovingPartsTime < Time.time)
                    UpdateMovingParts();
            }
            else
                m_UpdateMovingParts = false;
        }

        private void UpdateMovingParts()
        {
            for (int i = 0; i < m_MovingParts.Length; i++)
            {
                if (m_MovingParts[i].PositionOffset != Vector3.zero)
                {
                    Vector3 newPosition = new Vector3(
                            m_MovingParts[i].Transform.localPosition.x + m_MovingParts[i].PositionOffset.x,
                            m_MovingParts[i].Transform.localPosition.y + m_MovingParts[i].PositionOffset.y,
                            m_MovingParts[i].Transform.localPosition.z + m_MovingParts[i].PositionOffset.z);

                    m_MovingParts[i].Transform.localPosition = newPosition;
                }

                if (m_MovingParts[i].RotationOffset != Vector3.zero)
                {
                    Vector3 originalEulerAngles = m_MovingParts[i].Transform.localEulerAngles;

                    Vector3 newEulerAngles = new Vector3(
                            originalEulerAngles.x + m_MovingParts[i].RotationOffset.x,
                            originalEulerAngles.y + m_MovingParts[i].RotationOffset.y,
                            originalEulerAngles.z + m_MovingParts[i].RotationOffset.z);

                    m_MovingParts[i].Transform.localEulerAngles = newEulerAngles;
                }
            }
        }
    }
}
