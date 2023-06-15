using IdenticalStudios.InventorySystem;
using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof(ItemContainerUI))]
    public sealed class ItemContainerWeightUI : PlayerUIBehaviour
    {
        [SerializeField, Range(0, 5)]
        private int m_Decimals = 1;

        [SerializeField]
        private FillBarUI m_WeightBar;

        [SerializeField]
        private TextMeshProUGUI m_WeightText;

        private ContainerWeightRestriction m_ContainerWeight;
        

        protected override void OnAttachment()
        {
            var container = GetComponent<ItemContainerUI>();
            container.onContainerAttached += OnContainerAttached;
        }

        protected override void OnDetachment()
        {
            var container = GetComponent<ItemContainerUI>();
            container.onContainerAttached -= OnContainerAttached;
        }

        private void OnContainerAttached(IItemContainer container)
        {
            if (m_ContainerWeight != null)
                m_ContainerWeight.WeightChanged -= OnWeightChanged;

            if (container != null && container.TryGetContainerRestriction(out m_ContainerWeight))
            {
                m_ContainerWeight.WeightChanged += OnWeightChanged;
                OnWeightChanged(m_ContainerWeight.TotalWeight);
            }
            else
                m_ContainerWeight = null;
        }

        private void OnWeightChanged(float totalWeight)
        {
            float maxWeight = m_ContainerWeight.MaxWeight;
            m_WeightText.text = $"{(float)System.Math.Round(totalWeight, m_Decimals)} / {maxWeight} KG";

            m_WeightBar.SetFillAmount(totalWeight / maxWeight);
        }
    }
}