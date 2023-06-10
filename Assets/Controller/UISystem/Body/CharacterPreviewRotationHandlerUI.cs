using UnityEngine;
using UnityEngine.EventSystems;

namespace IdenticalStudios.UISystem
{
    public sealed class CharacterPreviewRotationHandlerUI : MonoBehaviour
    {
        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private Transform m_Root;


        [SerializeField, Range(0.01f, 10f)]
        private float m_YRotationSpeed = 0.3f;

        [SerializeField]
        private bool m_InvertYDirection = true;


        [SerializeField, Range(0.01f, 10f)]
        private float m_XRotationSpeed = 0.3f;

        [SerializeField]
        private bool m_InvertXDirection = true;

        [SerializeField, Range(0f, 180f)]
        private float m_MaxXRotation = 15f;


        [SerializeField, Range(-100f, 100f)]
        private float m_CameraMoveSpeed = -10f;

        [SerializeField, Range(-100f, 100f)]
        private float m_MinCameraDistance = 5f;

        [SerializeField, Range(-100f, 100f)]
        private float m_MaxCameraDistance = 20f;

        private Vector3 m_RootEulerAngles;
        private float m_CameraOffset;


        public void OnDrag(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointerData)
            {
                m_RootEulerAngles.y += (pointerData.delta.x * m_YRotationSpeed * (m_InvertYDirection ? -1f : 1f));
                m_RootEulerAngles.x += (pointerData.delta.y * m_XRotationSpeed * (m_InvertXDirection ? -1f : 1f));
                m_RootEulerAngles.x = Mathf.Clamp(m_RootEulerAngles.x, -m_MaxXRotation, m_MaxXRotation);
                m_Root.localRotation = Quaternion.Euler(m_RootEulerAngles);
            }
        }

        public void OnScroll(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointerData)
            {
                m_CameraOffset += pointerData.scrollDelta.y * 0.01f * m_CameraMoveSpeed;
                m_CameraOffset = Mathf.Clamp(m_CameraOffset, m_MinCameraDistance, m_MaxCameraDistance);
                m_Root.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -m_CameraOffset);
            }
        }

        private void Start()
        {
            m_RootEulerAngles = m_Root.localEulerAngles;
            m_CameraOffset = m_MinCameraDistance;
            m_Root.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -m_CameraOffset);
        }
    }
}