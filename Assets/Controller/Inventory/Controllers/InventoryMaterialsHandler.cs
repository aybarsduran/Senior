using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    /// <summary>
    /// TODO: Refactor.
    /// Acts as a wrapper for adding inventory based materials (e.g. sticks, rope etc.) to a structure.
    /// </summary>
    public sealed class InventoryMaterialsHandler : CharacterBehaviour
    {
        private IItemContainer[] m_Containers;


        protected override void OnBehaviourEnabled()
        {
            m_Containers = Character.Inventory.GetContainersWithoutTag().ToArray();
        }

        public void AddMaterial(BuildablePreview preview, bool addAll)
        {
            for (int i = 0; i < m_Containers.Length; i++)
            {
                var container = m_Containers[i];
                for (int j = 0; j < m_Containers[i].Capacity; j++)
                {
                    var slot = container.Slots[j];
                    if (slot.HasItem && slot.Item.Definition.TryGetDataOfType(out BuildMaterialData buildData))
                    {
                        if (preview.TryAddBuildingMaterial(buildData.BuildMaterial))
                        {
                            slot.Item.StackCount--;

                            if (!addAll)
                                return;
                        }
                    }
                }
            }
        }
    }
}