using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class PlayerVitalsUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The energy bar image, the fill amount will be modified based on the current energy value.")]
        private Image m_EnergyBar;

        [SerializeField]
        [Tooltip("The thirst bar image, the fill amount will be modified based on the current thirst value.")]
        private Image m_ThirstBar;

        [SerializeField]
        [Tooltip("The hunger bar image, the fill amount will be modified based on the current hunger value.")]
        private Image m_HungerBar;

        private IEnergyManager m_Energy;
        private IThirstManager m_Thirst;
        private IHungerManager m_Hunger;


        protected override void OnAttachment()
        {
            GetModule(out m_Energy);
            GetModule(out m_Thirst);
            GetModule(out m_Hunger);
        }

        private void FixedUpdate()
        {
            if (Player == null)
                return;

            m_EnergyBar.fillAmount = m_Energy.Energy / m_Energy.MaxEnergy;
            m_ThirstBar.fillAmount = m_Thirst.Thirst / m_Thirst.MaxThirst;
            m_HungerBar.fillAmount = m_Hunger.Hunger / m_Hunger.MaxHunger;
        }
    }
}