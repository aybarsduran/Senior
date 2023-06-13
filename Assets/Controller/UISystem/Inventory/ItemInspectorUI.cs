using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class ItemInspectorUI : MonoBehaviour
    {
        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private ItemDefinitionSlotUI m_InfoHandler;


        private void UpdateShownSlot(ItemSlotUI slot)
        {
            if (slot == null || !slot.HasItem)
            {
                m_Panel.Show(false);
                return;
            }

            m_InfoHandler.SetItem(slot);
            m_Panel.Show(true);
        }

        private void OnEnable() => ItemSelectorUI.SelectedSlotChanged += UpdateShownSlot;
        private void OnDisable() => ItemSelectorUI.SelectedSlotChanged -= UpdateShownSlot;
    }
}
