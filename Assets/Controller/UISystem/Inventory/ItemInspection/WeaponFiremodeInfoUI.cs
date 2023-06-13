using IdenticalStudios.InventorySystem;
using IdenticalStudios.WieldableSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class WeaponFiremodeInfoUI : PlayerDataInfoUI<IItem>
    {
        [SerializeField]
        private TextMeshProUGUI m_FiremodeText;

        [SerializeField]
        private Image m_FiremodeImg;

        private IWieldableSelectionHandler m_SelectionHandler;
        private FirearmIndexBasedAttachmentsManager m_FiremodesManager;


        protected override void OnAttachment() => Player.GetModule(out m_SelectionHandler);

        protected override bool CanEnableInfo()
        {
            var wieldable = m_SelectionHandler.GetWieldableItemWithId(data.Id);

            if (wieldable != null)
                return wieldable.gameObject.TryGetComponent(out m_FiremodesManager);

            return false;
        }

        protected override void OnInfoUpdate()
        {
            if (m_FiremodesManager != null && m_FiremodesManager.CurrentMode != null)
            {
                if (m_FiremodeText != null)
                    m_FiremodeText.text = m_FiremodesManager.CurrentMode.Name;

                if (m_FiremodeImg != null)
                {
                    m_FiremodeImg.enabled = m_FiremodeImg.sprite != null;
                    m_FiremodeImg.sprite = m_FiremodesManager.CurrentMode.Icon;
                }
            }
        }

        protected override void OnInfoDisabled()
        {
            if (m_FiremodeText != null) m_FiremodeText.text = string.Empty;
            if (m_FiremodeImg != null) m_FiremodeImg.enabled = false;
        }
    }
}
