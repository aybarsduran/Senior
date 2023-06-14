using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class AnimationFeedbackUI : IInteractableFeedbackUI
    {
        [SerializeField]
        private Animator m_Animator;
        
        [SerializeField, AnimatorParameter]
        private string m_NormalTrigger;

        [SerializeField, AnimatorParameter]
        private string m_HighlightTrigger;

        [SerializeField, AnimatorParameter]
        private string m_SelectedTrigger;

        [SerializeField, AnimatorParameter]
        private string m_PressedTrigger;


        public void OnNormal(bool instant) => m_Animator.SetTrigger(m_NormalTrigger);
        public void OnHighlighted(bool instant) => m_Animator.SetTrigger(m_HighlightTrigger);
        public void OnSelected(bool instant) => m_Animator.SetTrigger(m_SelectedTrigger);
        public void OnPressed(bool instant) => m_Animator.SetTrigger(m_PressedTrigger);

#if UNITY_EDITOR
        public void OnValidate(SelectableUI selectable)
        {
            m_Animator = selectable.GetComponent<Animator>();
        }
#endif
    }
}