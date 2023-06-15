using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    public sealed class HighlightGraphicFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Image m_SelectionGraphic;

        [SerializeField]
        private Color m_SelectedColor;

        [SerializeField]
        private Color m_HighlightedColor;


        public void OnNormal(bool instant)
        {
            m_SelectionGraphic.enabled = false;
        }

        public void OnHighlighted(bool instant)
        {
            m_SelectionGraphic.enabled = true;
            m_SelectionGraphic.color = m_HighlightedColor;
        }

        public void OnSelected(bool instant)
        {
            m_SelectionGraphic.enabled = true;
            m_SelectionGraphic.color = m_SelectedColor;
        }

        public void OnPressed(bool instant)
        {
        }

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable) { }
#endif
    }
}