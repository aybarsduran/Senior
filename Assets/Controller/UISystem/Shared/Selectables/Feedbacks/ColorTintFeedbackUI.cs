using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    public sealed class ColorTintFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Graphic m_TargetGraphic;

        [SerializeField]
        private Color m_NormalColor = Color.grey;
        
        [SerializeField]
        private Color m_HighlightedColor = Color.grey;

        [SerializeField]
        private Color m_SelectedColor = Color.grey;

        [SerializeField]
        private Color m_PressedColor = Color.grey;

        [SerializeField, Range(0.01f, 1f)]
        private float m_FadeDuration = 0.1f;


        public void OnNormal(bool instant)
        {
            if (instant)
                m_TargetGraphic.CrossFadeColor(m_NormalColor, 0f, true, true);
            else
                m_TargetGraphic.CrossFadeColor(m_NormalColor, m_FadeDuration, true, true);
        }

        public void OnHighlighted(bool instant)
        {
            if (instant)
                m_TargetGraphic.CrossFadeColor(m_HighlightedColor, 0f, true, true);
            else
                m_TargetGraphic.CrossFadeColor(m_HighlightedColor, m_FadeDuration, true, true);
        }

        public void OnSelected(bool instant)
        {
            if (instant)
                m_TargetGraphic.CrossFadeColor(m_SelectedColor, 0f, true, true);
            else
                m_TargetGraphic.CrossFadeColor(m_SelectedColor, m_FadeDuration, true, true);
        }

        public void OnPressed(bool instant)
        {
            if (instant)
                m_TargetGraphic.CrossFadeColor(m_PressedColor, 0f, true, true);
            else
                m_TargetGraphic.CrossFadeColor(m_PressedColor, m_FadeDuration, true, true);
        }

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable)
        {
            m_TargetGraphic = selectable.GetComponent<Graphic>();

            if (selectable.TryGetComponent(out CanvasRenderer canRenderer))
                canRenderer.SetColor(m_NormalColor);
        }
#endif
    }
}