using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    public sealed class SpriteSwapFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Image m_Image;

        [SerializeField, HideInInspector]
        private Sprite m_NormalSprite;

        [SerializeField]
        private Sprite m_HighlightedSprite;

        [SerializeField]
        private Sprite m_SelectedSprite;

        [SerializeField]
        private Sprite m_PressedSprite;
        
        
        public void OnNormal(bool instant) => m_Image.sprite = m_NormalSprite;
        public void OnHighlighted(bool instant) => m_Image.sprite = m_HighlightedSprite;
        public void OnSelected(bool instant) => m_Image.sprite = m_SelectedSprite;
        public void OnPressed(bool instant) => m_Image.sprite = m_PressedSprite;

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable)
        {
            if (selectable.TryGetComponent(out m_Image))
                m_Image.sprite = m_NormalSprite;
        }
#endif
    }
}