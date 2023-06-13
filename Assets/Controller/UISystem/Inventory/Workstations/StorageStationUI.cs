using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class StorageStationUI : WorkstationInspectorBaseUI<StorageStation>
    {
        [Title("Settings (Storage")]

        [SerializeField]
        private ItemContainerUI m_ItemContainer;


        protected override void OnInspectionStarted(StorageStation workstation)
        {
            m_ItemContainer.AttachToContainer(workstation.ItemContainer);
        }

        protected override void OnInspectionEnded(StorageStation workstation)
        {
            m_ItemContainer.DetachFromContainer();
        }
    }
}