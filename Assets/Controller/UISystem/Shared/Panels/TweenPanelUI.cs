using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu("IdenticalStudios/UserInterface/Panels/Tween Panel")]
    public class TweenPanelUI : AudioPanelUI
    {
        [Title("Animation")]

        [SerializeField]
        private TweenSequence m_ShowAnimation;

        [SerializeField]
        private TweenSequence m_HideAnimation;


        protected override void ShowPanel()
        {
            base.ShowPanel();

            m_HideAnimation.CancelAnimation();
            m_ShowAnimation.PlayAnimation();
        }

        protected override void HidePanel()
        {
            base.HidePanel();

            m_ShowAnimation.CancelAnimation();
            m_HideAnimation.PlayAnimation();
        }

        private void OnDestroy()
        {
            m_ShowAnimation.CancelAnimation();
            m_HideAnimation.CancelAnimation();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_ShowAnimation?.OnValidate(gameObject);
            m_HideAnimation?.OnValidate(gameObject);
        }
#endif
    }
}
