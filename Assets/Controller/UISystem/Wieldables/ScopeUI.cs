using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class ScopeUI : MonoBehaviour
	{
        [SerializeField]
        [Tooltip("The canvas group used to fade the scopes in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0f, 5f)]
        private float m_ShowDuration = 0.3f;

        [SerializeField, Range(0f, 5f)]
        private float m_HideDuration = 0.2f;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        [Tooltip("All of the existing UI scopes.")]
		private GameObject[] m_Scopes;

        private Tween<float> m_Tween;
        private int m_CurrentScopeIndex = -1;
        private bool m_ScopeEnabled = false;
        private int m_ScopeToDisable = -1;

        private static ScopeUI s_Instance;


        private void OnEnable()
        {
            foreach (var scope in m_Scopes)
                scope.SetActive(false);

            m_CanvasGroup.alpha = 0f;
            m_Tween = m_CanvasGroup.TweenCanvasGroupAlpha(0f, m_ShowDuration)
                .SetCompleteCallback(OnCanvasAlphaLerped)
                .SetCancelCallback(OnCanvasAlphaLerped)
                .SetEase(EaseType.CubicInOut);

            s_Instance = this;
        }

        private void OnDisable()
        {
            foreach (var scope in m_Scopes)
                scope.SetActive(false);
            
            if (s_Instance == this)
                s_Instance = null;
        }

        public static void EnableScope(int scopeIndex, float enableDelay = 0.5f) 
        {
            if (s_Instance != null)
                s_Instance.EnableScopeUI(scopeIndex, enableDelay);
        }

        public static void DisableScope()
        {
            if (s_Instance != null)
                s_Instance.DisableScopeUI();
        }

        private void OnCanvasAlphaLerped()
        {
            if (!m_ScopeEnabled && m_ScopeToDisable != -1)
                m_Scopes[m_ScopeToDisable].SetActive(false);
        }

        private void EnableScopeUI(int index, float enableDelay = 0.5f) 
        {
            m_ScopeEnabled = true;

            int newIndex = Mathf.Clamp(index, 0, m_Scopes.Length - 1);

            // Change current scope
            if (newIndex != m_CurrentScopeIndex)
            {
                m_Tween.Cancel();

                m_Scopes[newIndex].SetActive(true);
                m_CurrentScopeIndex = newIndex;

                m_Tween.SetTargetValue(1f).SetDuration(m_ShowDuration).SetDelay(enableDelay).Play();
            }
        }

        private void DisableScopeUI()
        {
            m_ScopeEnabled = false;
            m_ScopeToDisable = m_CurrentScopeIndex;
            m_CurrentScopeIndex = -1;

            m_Tween.SetTargetValue(0f).SetDuration(m_HideDuration).SetDelay(0f).Play();
        }
    }
}