using System;
using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class ItemInfoUI : ItemInfoBasicUI
    {
        [Title("Stack")]
        
        [SerializeField]
        private GameObject m_StackObject;
        
        [SerializeField]
        private TextMeshProUGUI m_StackTxt;
        
        [Title("Weight")]
        
        [SerializeField]
        private TextMeshProUGUI m_WeightTxt;
        
        [SerializeField]
        private string m_WeightSuffix = "KG";
        
        
        protected override void OnInfoUpdate()
        {
            base.OnInfoUpdate();

            if (m_StackTxt != null)
            {
                if (m_StackObject != null)
                    m_StackObject.SetActive(data.StackCount > 1);

                m_StackTxt.text = data.StackCount > 1 ? ("x" + data.StackCount) : string.Empty;
            }

            if (m_WeightTxt != null)
                m_WeightTxt.text = $"{Math.Round(data.Definition.Weight * data.StackCount, 3)} {m_WeightSuffix}";
        }

        protected override void OnInfoDisabled() 
        {
            base.OnInfoDisabled();

            if (m_StackTxt != null)
            {
                if (m_StackObject != null)
                    m_StackObject.SetActive(false);

                m_StackTxt.text = string.Empty;
            }

            if (m_WeightTxt != null)
                m_WeightTxt.text = string.Empty;
        }
    }
}