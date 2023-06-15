using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu(k_AddMenuPath + "Item Property Fill Bar Info")]
    public sealed class ItemPropertyFillBarInfoUI : DataInfoUI<IItem>
    {
        #region Internal
        [System.Serializable]
        private class FillBarProperty
        {
            public DataIdReference<ItemPropertyDefinition> Property;

            [SpaceArea]

            public FillBarUI FillBar;

            [Range(0f, 1000f)]
            public float MinValue = 0f;

            [Range(0f, 1000f)]
            public float MaxValue = 1f;
        }
        #endregion

        [SerializeField, ReorderableList(childLabel: "Fill Bar Property")]
        private FillBarProperty[] m_Properties;


        protected override bool CanEnableInfo()
        {
            foreach (var fillProperty in m_Properties)
                fillProperty.FillBar.SetActive(true);

            return true;
        }

        protected override void OnInfoUpdate()
        {
            foreach (var fillProperty in m_Properties)
            {
                if (data.TryGetPropertyWithId(fillProperty.Property, out var prop))
                {
                    if (prop.Type != ItemPropertyType.Integer && prop.Type != ItemPropertyType.Float)
                        continue;

                    float value = prop.Float;
                    value = Mathf.Clamp(value, fillProperty.MinValue, fillProperty.MaxValue);
                    float fillAmount = value / fillProperty.MaxValue;
                    fillProperty.FillBar.SetFillAmount(fillAmount);
                    fillProperty.FillBar.SetActive(true);
                }
                else
                    fillProperty.FillBar.SetActive(false);
            }
        }

        protected override void OnInfoDisabled()
        {
            foreach (var fillProperty in m_Properties)
                fillProperty.FillBar.SetActive(false);
        }
    }
}
