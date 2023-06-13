using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class GenericInteractionUI : MonoBehaviour, IInteractableInfoDisplayer
    {
        public IEnumerable<Type> HoverableTypes => m_InteractableTypes;
        private static readonly Type[] m_InteractableTypes = { typeof(IInteractable) };

        [SerializeField]
        [Tooltip("The UI panel used in showing / hiding the underlying images.")]
        private PanelUI m_Panel;

        [SpaceArea]

		[SerializeField]
        [Tooltip("A UI text component that's used for displaying the current interactable's name.")]
        private TextMeshProUGUI m_InteractableNameText;

        [SerializeField]
        [Tooltip("An image that separate the name text from the description text (optional). " +
                "It gets disabled when the current interactable doesn't have a description.")]
        private Image m_Separator;

        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current interactable's description.")]
        private TextMeshProUGUI m_DescriptionText;

        [SerializeField]
        [Tooltip("An image that used in showing the time the current interactable has been interacted with.")]
        private Image m_InteractProgressImg;

        private IHoverable m_AttachedHoverable;


        public void ShowInfo(IHoverable hoverable)
        {
            if (!string.IsNullOrEmpty(hoverable.Title))
                m_Panel.Show(true);
        }

        public void HideInfo() => m_Panel.Show(false);

        public void UpdateInfo(IHoverable hoverable)
        {
            if (m_AttachedHoverable != null)
                m_AttachedHoverable.DescriptionChanged -= OnDescriptionChanged;

            m_AttachedHoverable = hoverable;

            if (m_AttachedHoverable == null)
                return;
            
            m_InteractableNameText.text = m_AttachedHoverable.Title;
            m_AttachedHoverable.DescriptionChanged += OnDescriptionChanged;
            OnDescriptionChanged();
        }

        public void SetInteractionProgress(float progress) => m_InteractProgressImg.fillAmount = progress;
        private void Start() => SetInteractionProgress(0f);

        private void OnDescriptionChanged()
        {
            m_DescriptionText.text = m_AttachedHoverable.Description;

            if (m_Separator != null)
                m_Separator.enabled = !string.IsNullOrEmpty(m_DescriptionText.text);
        }
    }
}