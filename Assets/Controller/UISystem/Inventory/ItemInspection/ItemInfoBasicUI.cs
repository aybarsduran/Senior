using IdenticalStudios.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu(k_AddMenuPath + "Item Basic Info")]
    public class ItemInfoBasicUI : DataInfoUI<IItem>
    {
        public Image IconImage => m_IconImg;
        public Image BGIconImage => m_BGIconImg;

        [Title("General")]

        [SerializeField]
        private TextMeshProUGUI m_NameTxt;

        [SerializeField]
        private Image m_IconImg;

        [SerializeField]
        private Image m_BGIconImg;

        [SerializeField]
        private TextMeshProUGUI m_DescriptionTxt;


        protected override bool CanEnableInfo()
        {
            if (m_IconImg != null)
                m_IconImg.enabled = true;

            if (m_BGIconImg != null)
                m_BGIconImg.enabled = false;

            return true;
        }

        protected override void OnInfoUpdate()
        {
            var itemInfo = data.Definition;

            if (m_NameTxt != null)
                m_NameTxt.text = itemInfo.Name;

            if (m_DescriptionTxt != null)
                m_DescriptionTxt.text = itemInfo.Description;

            if (m_IconImg != null)
                m_IconImg.sprite = itemInfo.Icon;
        }

        protected override void OnInfoDisabled()
        {
            if (m_BGIconImg != null)
                m_BGIconImg.enabled = true;

            if (m_NameTxt != null)
                m_NameTxt.text = string.Empty;

            if (m_DescriptionTxt != null)
                m_DescriptionTxt.text = string.Empty;

            if (m_IconImg != null)
                m_IconImg.enabled = false;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (Application.isPlaying || m_IconImg == null)
                return;

            m_IconImg.enabled = m_IconImg.sprite != null;

            if (m_BGIconImg != null)
                m_BGIconImg.enabled = m_BGIconImg.sprite != null;
        }
#endif
    }
}
