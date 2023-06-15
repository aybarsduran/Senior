using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public sealed class ChildOfLookHandler : CharacterBehaviour
    {
        public event UnityAction OnMove;

        [SerializeField, NotNull]
        private Transform m_Parent;

        [SpaceArea]

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

            StartCoroutine(C_DelayedLookHandlerListen());
        }

        protected override void OnBehaviourDisabled()
        {
            if (m_Parent == null)
            {
                Debug.LogError("No parent!");
                return;
            }

            GetModule<ILookHandler>().PostViewUpdate -= UpdatePosition;
        }

        private void UpdatePosition()
        {
            var position = m_Parent.position + m_Parent.TransformVector(m_PositionOffset);
            var rotation = m_Parent.rotation * Quaternion.Euler(m_RotationOffset);
            transform.SetPositionAndRotation(position, rotation);

            OnMove?.Invoke();
        }

        private IEnumerator C_DelayedLookHandlerListen()
        {
            yield return null;
            GetModule<ILookHandler>().PostViewUpdate += UpdatePosition;
        }
    }
}