using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class ItemRequirementInfoUI : PlayerDataInfoUI<IItem>
    {
        [SerializeField, ReorderableList(HasLabels = false)]
        private RequirementUI[] m_Requirements;

        [SerializeField]
        private Color m_HasEnoughColor = Color.white;

        [SerializeField]
        private Color m_NotEnoughColor = Color.red;
        
        
        protected override bool CanEnableInfo()
        {
            bool enableInfo = data != null;

            if (enableInfo)
                gameObject.SetActive(true);
            
            return enableInfo;
        }

        protected override void OnInfoUpdate()
        {
            var def = data.Definition;

            if (!def.TryGetDataOfType<CraftingData>(out var craftData))
                return;
            
            var blueprint = craftData.Blueprint;

            for (int i = 0; i < m_Requirements.Length; i++)
            {
                if (i > blueprint.Length - 1)
                {
                    m_Requirements[i].gameObject.SetActive(false);
                    continue;
                }

                m_Requirements[i].gameObject.SetActive(true);

                if (blueprint[i].Item.IsNull)
                    continue;
                    
                CraftRequirement requirement = blueprint[i];
                ItemDefinition requiredItem = requirement.Item.Def;
                    
                int itemCount = Player.Inventory.GetItemsWithIdCount(requirement.Item);
                bool hasEnoughMaterials = itemCount >= requirement.Amount;
                m_Requirements[i].Display(requiredItem.Icon, "x" + requirement.Amount, hasEnoughMaterials ? m_HasEnoughColor : m_NotEnoughColor);
            }
        }

        protected override void OnInfoDisabled()
        {
            gameObject.SetActive(false);
        }
    }
}
