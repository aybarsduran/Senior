using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class PlayerHealthUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The health bar image, the fill amount will be modified based on the current health value.")]
        private Image m_HealthBar;

        private IHealthManager m_Health;


        protected override void OnAttachment()
        {
            if (TryGetModule(out m_Health))
            {
                m_Health.DamageTaken += UpdateHealthBar;
                m_Health.HealthRestored += UpdateHealthBar;
                UpdateHealthBar(0f);
            }
        }

        protected override void OnDetachment()
        {
            if (m_Health != null)
            {
                m_Health.DamageTaken -= UpdateHealthBar;
                m_Health.HealthRestored -= UpdateHealthBar;
            }
        }

        private void UpdateHealthBar(float change) => m_HealthBar.fillAmount = m_Health.Health / m_Health.MaxHealth;
    }
}
