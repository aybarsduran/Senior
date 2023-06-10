using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class CharacterPreviewUI : PlayerUIBehaviour
    {
        //References

        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private BodyClothing m_BodyClothing;

        //Equipment Containers

        [SerializeField]
        private string m_HeadContainerName = "Head";

        [SerializeField]
        private string m_TorsoContainerName = "Torso";

        [SerializeField]
        private string m_LegsContainerName = "Legs";

        [SerializeField]
        private string m_FeetContainerName = "Feet";


        private void OnClothingSlotChanged(ItemSlot itemSlot, ClothingType clothingType)
        {
            if (itemSlot.HasItem)
                m_BodyClothing.ShowClothing(itemSlot.Item.Id);
            else
                m_BodyClothing.HideClothing(clothingType);
        }

        protected override void OnAttachment()
        {
            var inventory = Player.Inventory;

            var headContainer = inventory.GetContainerWithName(m_HeadContainerName);
            var torsoContainer = inventory.GetContainerWithName(m_TorsoContainerName);
            var LegsContainer = inventory.GetContainerWithName(m_LegsContainerName);
            var FeetContainer = inventory.GetContainerWithName(m_FeetContainerName);

            headContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Head);
            torsoContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Torso);
            LegsContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Legs);
            FeetContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Feet);

            OnClothingSlotChanged(headContainer.Slots[0], ClothingType.Head);
            OnClothingSlotChanged(torsoContainer.Slots[0], ClothingType.Torso);
            OnClothingSlotChanged(LegsContainer.Slots[0], ClothingType.Legs);
            OnClothingSlotChanged(FeetContainer.Slots[0], ClothingType.Feet);

            m_Camera.enabled = false;
            m_Camera.forceIntoRenderTexture = true;

            if (TryGetModule(out IInventoryInspectManager inspection))
            {
                inspection.BeforeInspectionStarted += OnInspectionStarted;
                inspection.AfterInspectionEnded += OnInspectionEnded;
            }
        }

        protected override void OnDetachment()
        {
            if (TryGetModule(out IInventoryInspectManager inspection))
            {
                inspection.BeforeInspectionStarted -= OnInspectionStarted;
                inspection.AfterInspectionEnded -= OnInspectionEnded;
            }
        }

        private void OnInspectionEnded() => m_Camera.enabled = false;
        private void OnInspectionStarted() => m_Camera.enabled = true;
    }
}