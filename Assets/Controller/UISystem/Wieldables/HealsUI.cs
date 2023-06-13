using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class HealsUI : PlayerUIBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_HealsCountText;

        private IWieldableHealingHandler m_HealingHandler;


        protected override void OnAttachment()
        {
            GetModule(out m_HealingHandler);

            m_HealingHandler.HealsCountChanged += OnHealsCountChanged;
            OnHealsCountChanged();
        }

        protected override void OnDetachment()
        {
            if (m_HealingHandler != null)
                m_HealingHandler.HealsCountChanged -= OnHealsCountChanged;
        }

        private void OnHealsCountChanged()
        {
            int healsCount = m_HealingHandler.GetHealsCount();
            m_HealsCountText.text = healsCount.ToString();
        }
    }
}
