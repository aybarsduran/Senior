using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    public sealed class TweenFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private TweenSequence m_NormalTween;
        
        [SerializeField]
        private TweenSequence m_HighlightedTween;
        
        [SerializeField]
        private TweenSequence m_SelectedTween;
        
        [SerializeField]
        private TweenSequence m_PressedTween;
        
        
        public void OnNormal(bool instant) => m_NormalTween.PlayAnimation();
        public void OnHighlighted(bool instant) => m_HighlightedTween.PlayAnimation();
        public void OnSelected(bool instant) => m_SelectedTween.PlayAnimation();
        public void OnPressed(bool instant) => m_PressedTween.PlayAnimation();

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable)
        {
            if (selectable == null || selectable.gameObject == null)
                return;
            
            var gameObject = selectable.gameObject;
            m_NormalTween?.OnValidate(gameObject);
            m_HighlightedTween?.OnValidate(gameObject);
            m_SelectedTween?.OnValidate(gameObject);
            m_PressedTween?.OnValidate(gameObject);
        }
#endif
    }
}