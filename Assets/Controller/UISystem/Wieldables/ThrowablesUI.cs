using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class ThrowablesUI : PlayerUIBehaviour
    {
        [SerializeField]
        private Image m_ThrowableIcon;

        [SerializeField]
        private TextMeshProUGUI m_ThrowableCountText;

        private IWieldableThrowableHandler m_ThrowableHandler;


        protected override void OnAttachment()
        {
            GetModule(out m_ThrowableHandler);

            m_ThrowableHandler.ThrowableIndexChanged += OnThrowableIndexChanged;
            m_ThrowableHandler.ThrowableCountChanged += OnThrowableCountChanged;

            OnThrowableIndexChanged();
            OnThrowableCountChanged();
        }

        protected override void OnDetachment()
        {
            m_ThrowableHandler.ThrowableIndexChanged -= OnThrowableIndexChanged;
            m_ThrowableHandler.ThrowableCountChanged -= OnThrowableCountChanged;
        }

        private void OnThrowableIndexChanged()
        {
            var throwable = m_ThrowableHandler.GetThrowableAtIndex(m_ThrowableHandler.SelectedIndex);

            if (throwable != null)
                m_ThrowableIcon.sprite = throwable.DisplayIcon != null ? throwable.DisplayIcon : null;
        }

        private void OnThrowableCountChanged()
        {
            int throwablesCount = m_ThrowableHandler.GetThrowableCountAtIndex(m_ThrowableHandler.SelectedIndex);
            m_ThrowableCountText.text = throwablesCount.ToString();
        }
    }
}
