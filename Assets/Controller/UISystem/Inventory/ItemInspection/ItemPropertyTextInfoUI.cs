using IdenticalStudios.InventorySystem;
using System;
using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class ItemPropertyTextInfoUI : DataInfoUI<IItem>
    {
        #region Internal
        [Serializable]
        private class FillBarProperty
        {
            public DataIdReference<ItemPropertyDefinition> Property;

            [SpaceArea]

            [NotNull]
            public TextMeshProUGUI Text;

            [Range(0, 10)]
            public int Decimals = 2;
        }
        #endregion

        [SerializeField, ReorderableList(childLabel: "Text Property")]
        private FillBarProperty[] m_Properties;


        protected override bool CanEnableInfo()
        {
            foreach (var txtProperty in m_Properties)
                txtProperty.Text.enabled = true;

            return true;
        }

        protected override void OnInfoUpdate()
        {
            foreach (var txtProperty in m_Properties)
            {
                if (data.TryGetPropertyWithId(txtProperty.Property, out var prop))
                {
                    if (prop.Type != ItemPropertyType.Integer && prop.Type != ItemPropertyType.Float)
                        continue;

                    string str = Math.Round(prop.Float, 3).ToString();
                    txtProperty.Text.text = str;
                }
                else
                    txtProperty.Text.text = string.Empty;
            }
        }

        protected override void OnInfoDisabled()
        {
            foreach (var txtProperty in m_Properties)
                txtProperty.Text.enabled = false;
        }
    }
}
