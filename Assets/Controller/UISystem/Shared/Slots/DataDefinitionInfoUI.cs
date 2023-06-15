using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public abstract class DataDefinitionInfoUI<T> : DataInfoUI<T> where T : DataDefinition<T>
    {
        [Title("General")]

        [SerializeField]
        protected TextMeshProUGUI m_NameText;

        [SerializeField]
        protected TextMeshProUGUI m_DescriptionText;

        [SerializeField]
        protected Image m_IconImg;


        protected override bool CanEnableInfo() => true;

        protected override void OnInfoUpdate()
        {
            if (m_NameText != null)
                m_NameText.text = data.Name;

            if (m_DescriptionText != null)
                m_DescriptionText.text = data.Description;

            if (m_IconImg != null)
            {
                m_IconImg.enabled = true;
                m_IconImg.sprite = data.Icon;
            }
        }

        protected override void OnInfoDisabled()
        {
            if (m_NameText != null)
                m_NameText.text = string.Empty;

            if (m_DescriptionText != null)
                m_DescriptionText.text = string.Empty;

            if (m_IconImg != null)
                m_IconImg.enabled = false;
        }
    }
}
