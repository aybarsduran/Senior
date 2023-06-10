using TMPro;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class SelectableTextUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_Text;

        [SerializeField, Range(0, 100)]
        private int m_SelectedFontSize = 15;

        [SerializeField]
        private Color m_SelectedTextColor = Color.white;

        private float m_OriginalFontSize;
        private Color m_OriginalTextColor;
        private FontStyles m_OriginalFontStyle;


        private void Awake()
        {
            InitializeNameText();
        }

        public void SelectText()
        {
            m_Text.fontStyle = FontStyles.Bold;
            m_Text.fontSize = m_SelectedFontSize;
            m_Text.color = m_SelectedTextColor;
        }

        public void DeselectText()
        {
            m_Text.fontStyle = m_OriginalFontStyle;
            m_Text.fontSize = m_OriginalFontSize;
            m_Text.color = m_OriginalTextColor;
        }

        private void InitializeNameText()
        {
            m_OriginalFontSize = m_Text.fontSize;
            m_OriginalTextColor = m_Text.color;
            m_OriginalFontStyle = m_Text.fontStyle;
        }
    }
}
