using IdenticalStudios;
using UnityEngine;

namespace IdenticalStudios
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/wieldable#player-look-follow-behaviour")]
    public sealed class PlayerLookFollow : CharacterBehaviour
    {
        [SerializeField]
        private Transform m_Parent;

        [SerializeField]
        private bool m_CustomOffset;

        [SerializeField]
        private Vector3 m_PositionOffset;

        [SerializeField]
        private Vector3 m_RotationOffset;


        protected override void OnBehaviourEnabled()
        {
            if (m_Parent == null)
            {
                Debug.LogError("No parent!");
                return;
            }

            if (!m_CustomOffset)
            {
                m_PositionOffset = m_Parent.InverseTransformPoint(transform.position);
                m_RotationOffset = (Quaternion.Inverse(m_Parent.rotation) * transform.rotation).eulerAngles;
            }

            Character.GetModule<ILookHandler>().PostViewUpdate += UpdatePosition;
        }

        private void UpdatePosition()
        {
            transform.position = m_Parent.position + m_Parent.TransformVector(m_PositionOffset);
            transform.rotation = m_Parent.rotation * Quaternion.Euler(m_RotationOffset);
        }
    }
}