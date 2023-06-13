using IdenticalStudios.ProceduralMotion;
using IdenticalStudios.WieldableSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class FiremodesUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the currently selected fire mode.")]
        private TextMeshProUGUI m_FiremodeNameText;

        [SerializeField]
        private Image m_FiremodeIconImage;

        [Title("Animation")]

        [SerializeField]
        private TweenSequence m_ShowAnimation;

        [SerializeField]
        private TweenSequence m_UpdateAnimation;

        [SerializeField]
        private TweenSequence m_HideAnimation;

        private IWieldablesController m_WieldableController;
        private FirearmIndexBasedAttachmentsManager m_IndexAttachments;


        protected override void OnAttachment()
        {
            GetModule(out m_WieldableController);
            m_WieldableController.WieldableEquipStopped += OnWieldableEquipped;

            m_HideAnimation.PlayAnimation();
        }

        private void OnWieldableEquipped(IWieldable wieldable)
        {
            // Unsubscribe from previous index-based attachments handler
            if (m_IndexAttachments != null)
            {
                m_IndexAttachments.ModeChanged -= OnAttachmentIndexChanged;
                m_IndexAttachments = null;

                m_HideAnimation.PlayAnimation();
            }

            // Subscribe to current index-based attachments handler
            if (wieldable != null && wieldable.gameObject.TryGetComponent(out m_IndexAttachments))
            {
                m_IndexAttachments.ModeChanged += OnAttachmentIndexChanged;

                if (m_FiremodeNameText != null)
                    m_FiremodeNameText.text = m_IndexAttachments.CurrentMode.Name;

                if (m_FiremodeIconImage != null)
                    m_FiremodeIconImage.sprite = m_IndexAttachments.CurrentMode.Icon;

                m_ShowAnimation.PlayAnimation();
            }
            else
                m_IndexAttachments = null;
        }

        private void OnAttachmentIndexChanged() 
        {
            if (m_FiremodeNameText != null)
                m_FiremodeNameText.text = m_IndexAttachments.CurrentMode.Name;

            if (m_FiremodeIconImage != null)
                m_FiremodeIconImage.sprite = m_IndexAttachments.CurrentMode.Icon;

            m_UpdateAnimation.PlayAnimation();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_ShowAnimation?.OnValidate(gameObject);
            m_UpdateAnimation?.OnValidate(gameObject);
            m_HideAnimation?.OnValidate(gameObject);
        }
#endif
    }
}