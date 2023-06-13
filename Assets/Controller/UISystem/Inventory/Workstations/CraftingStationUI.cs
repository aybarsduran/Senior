using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class CraftingStationUI : WorkstationInspectorBaseUI<CraftStation>
    {
        [Title("Settings (Crafting)")]

        [SerializeField]
        private CraftingUI m_CraftingUI;


        protected override void OnInspectionStarted(CraftStation workstation)
        {
            m_CraftingUI.SetAvailableCraftingLevels(workstation.CraftableLevels);
        }

        protected override void OnInspectionEnded(CraftStation workstation)
        {
            m_CraftingUI.ResetCraftingLevel();
        }
    }
}
