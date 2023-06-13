using IdenticalStudios.BuildingSystem;
using IdenticalStudios.InventorySystem;
using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class RepairStationUI : WorkstationInspectorBaseUI<RepairStation>
    {
        [Title("Settings (Repairing)")]

        [SerializeField, SceneObjectOnly]
        private ItemContainerUI m_ContainerUI;

        [SerializeField, SceneObjectOnly]
        private SelectableUI m_RepairButton;

        [Title("Required Items")]

        [SerializeField, SceneObjectOnly]
        private GameObject m_RequiredItemsRoot;

        [SerializeField, PrefabObjectOnly]
        private RequirementUI m_RequiredItemTemplate;

        [SerializeField]
        private Color m_EnoughItemsColor = Color.gray;

        [SerializeField]
        private Color m_NotEnoughItemsColor = new(0.7f, 0f, 0f, 0.7f);

        [Title("Message")]

        [SerializeField, SceneObjectOnly]
        private TextMeshProUGUI m_MessageText;

        [SerializeField]
        private string m_PlaceItemText = "Place item to repair...";

        [SerializeField]
        private string m_CanNotRepairItemText = "Can't repair this item...";

        [SerializeField]
        private string m_RequiredItemsText = "Required Items";

        private RequirementUI[] m_RequiredItemUIs;


        protected override void OnInspectionStarted(RepairStation workstation)
        {
            m_ContainerUI.AttachToContainer(workstation.GetContainers()[0]);

            workstation.ItemToRepairChanged += UpdateUI;
            Player.Inventory.InventoryChanged += UpdateUI;

            UpdateUI();
        }

        protected override void OnInspectionEnded(RepairStation workstation)
        {
            m_ContainerUI.DetachFromContainer();

            workstation.ItemToRepairChanged -= UpdateUI;
            Player.Inventory.InventoryChanged -= UpdateUI;
        }

        protected override void OnAttachment()
        {
            m_MessageText.gameObject.SetActive(true);

            m_RequiredItemUIs = new RequirementUI[4];

            for (int i = 0; i < m_RequiredItemUIs.Length; i++)
            {
                m_RequiredItemUIs[i] = Instantiate(m_RequiredItemTemplate, m_RequiredItemsRoot.transform);
                m_RequiredItemUIs[i].gameObject.SetActive(false);
            }

            m_RepairButton.OnSelected += OnRepairBtnClicked;
        }

        private void OnRepairBtnClicked(SelectableUI btn)
        {
            if (m_Workstation.RepairDuration > 0.01f)
            {
                var repairStartParams = new CustomActionManagerUI.AParams("Item Repair", "Repairing Item...", m_Workstation.RepairDuration, true, OnRepairDone, null);
                CustomActionManagerUI.TryStartAction(repairStartParams);
            }
            else
                OnRepairDone();
        }

        private void OnRepairDone()
        {
            m_Workstation.RepairItem(Player);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (m_Workstation == null)
                return;

            bool hasItemToRepair = m_Workstation.ItemToRepair != null;
            bool canRepairItem = m_Workstation.CanRepairItem();

            if (hasItemToRepair)
            {
                if (canRepairItem)
                    m_MessageText.text = m_RequiredItemsText;
                else
                    m_MessageText.text = m_CanNotRepairItemText;
            }
            else
                m_MessageText.text = m_PlaceItemText;

            m_RequiredItemsRoot.SetActive(canRepairItem);

            bool hasEnoughItems = true;

            if (canRepairItem)
            {
                for (int i = 0; i < m_RequiredItemUIs.Length; i++)
                {
                    if (m_Workstation.RepairRequirementsForCurrentItem.Count > i)
                    {
                        m_RequiredItemUIs[i].gameObject.SetActive(true);

                        CraftRequirement requirement = m_Workstation.RepairRequirementsForCurrentItem[i];
                        var itemDef = requirement.Item.Def;

                        if (itemDef != null)
                        {
                            bool weHaveEnough = Player.Inventory.GetItemsWithIdCount(requirement.Item) >= requirement.Amount;

                            if (!weHaveEnough)
                                hasEnoughItems = false;

                            m_RequiredItemUIs[i].Display(itemDef.Icon, itemDef.Name + " x" + requirement.Amount, weHaveEnough ? m_EnoughItemsColor : m_NotEnoughItemsColor);
                        }
                    }
                    else
                        m_RequiredItemUIs[i].gameObject.SetActive(false);
                }
            }

            m_RepairButton.IsSelectable = canRepairItem && hasEnoughItems;
        }
    }
}