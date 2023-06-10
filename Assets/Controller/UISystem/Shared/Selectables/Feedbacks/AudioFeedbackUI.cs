using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    public sealed class AudioFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private SoundPlayer m_HighlightSound;

        [SerializeField]
        private SoundPlayer m_SelectedSound;

        
        public void OnNormal(bool instant) {}

        public void OnHighlighted(bool instant)
        {
            if (!instant)
                m_HighlightSound.Play2D();
        }

        public void OnSelected(bool instant)
        {
            if (!instant)
                m_SelectedSound.Play2D();
        }

        public void OnPressed(bool instant) {}

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable) {}
#endif
    }
}