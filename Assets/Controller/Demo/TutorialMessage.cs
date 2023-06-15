using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class TutorialMessage : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)]
        private float m_TransitionSpeed = 1f;

        [SerializeField, Range(0f, 100f)]
        private float m_MaxDistance;

        private TextMeshPro[] m_TextMeshes;
        private Color[] m_TextMeshColors;


        private void Start()
        {
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>();
            m_TextMeshColors = new Color[m_TextMeshes.Length];

            for (int i = 0; i < m_TextMeshes.Length; i++)
                m_TextMeshColors[i] = m_TextMeshes[i].color;
        }

        private void LateUpdate()
        {
            if (UnityUtils.CachedMainCamera == null)
                return;

            float angle = Vector3.Angle(gameObject.transform.forward, UnityUtils.CachedMainCamera.transform.forward);
            float distance = Vector3.Distance(UnityUtils.CachedMainCamera.transform.position, transform.position);

            UpdateTextAlpha(angle < 90 && distance < m_MaxDistance);
        }

        private void UpdateTextAlpha(bool enable)
        {
            for (int i = 0; i < m_TextMeshes.Length; i++)
                m_TextMeshes[i].CrossFadeAlpha(enable ? 1f : 0f, Time.deltaTime * m_TransitionSpeed, true);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (m_TextMeshes == null || m_TextMeshes.Length == 0)
                m_TextMeshes = GetComponentsInChildren<TextMeshPro>();
        }
#endif
    }
}
