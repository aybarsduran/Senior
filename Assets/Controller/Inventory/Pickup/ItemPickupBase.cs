using IdenticalStudios.InventorySystem;
using IdenticalStudios;
using IdenticalStudios.UISystem;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

/// <summary>
/// Item pickup base class.
/// </summary>
namespace IdenticalStudios.InventorySystem
{
    public abstract class ItemPickupBase : Interactable
    {
        private enum AddType
        {
            AddToMatchingContainer,
            AddToFirstEmptyContainer
        }

        [SerializeField]
        private AddType m_AddType;

        [SerializeField]
        [Tooltip("Sound that will be played upon picking the item up.")]
        private SoundPlayer m_PickUpSound;


        public abstract void LinkWithItem(IItem item);

        protected void PickUpItem(ICharacter character, IItem item, bool playPickUpSound = true)
        {
            if (item == null)
            {
                Debug.LogError("Item Instance is null, can't pick up anything.");
                return;
            }

            int addedCount = 0;
            int originalCount = item.StackCount;

            string rejectReason = string.Empty;

            if (m_AddType == AddType.AddToMatchingContainer)
            {
                var containers = character.Inventory.GetContainersWithTag(item.Definition.Tag);
                foreach (var container in containers)
                {
                    if (container.GetAllowedCount(item, item.StackCount, out rejectReason) > 0)
                        addedCount += container.AddItem(item);

                    if (addedCount == originalCount)
                        break;
                }

                if (originalCount > 1)
                    item.StackCount = originalCount - addedCount;
            }

            if (addedCount != originalCount)
            {
                var containers = character.Inventory.Containers;
                foreach (var container in containers)
                {
                    if (container.GetAllowedCount(item, item.StackCount, out rejectReason) > 0)
                        addedCount += container.AddItem(item);

                    if (addedCount == originalCount)
                        break;
                }
            }

            if (addedCount > 0)
            {
                string pickupUpMessage = $"Pickup up {item.Name}";

                if (addedCount > 1)
                    pickupUpMessage += $" x {addedCount}";

                MessageDisplayerUI.PushMessage(pickupUpMessage, item.Definition.Icon);

                if (playPickUpSound)
                    m_PickUpSound.Play2D(1f, SelectionType.Random);

                if (addedCount >= originalCount)
                    Destroy(gameObject);
            }
            else
            {
                MessageDisplayerUI.PushMessage(rejectReason != string.Empty ? rejectReason : "Inventory Full", Color.red);
            }
        }
    }
}
