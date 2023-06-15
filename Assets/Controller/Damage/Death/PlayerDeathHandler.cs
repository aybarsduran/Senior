using System.Collections;
using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
    /// Deals with the Player death and respawn behaviour.
    /// </summary>
    public sealed class PlayerDeathHandler : CharacterBehaviour
    {
        private enum ItemDropType { All, Equipped, None }

        [SerializeField]
        [Help("Items to drop on death.", UnityMessageType.None)]
        private ItemDropType m_ItemDropType = ItemDropType.None;


        protected override void OnBehaviourEnabled()
        {
            Character.HealthManager.Death += OnPlayerDeath;
            Character.HealthManager.Respawn += OnPlayerRespawn;
        }

        protected override void OnBehaviourDisabled()
        {
            Character.HealthManager.Death -= OnPlayerDeath;
            Character.HealthManager.Respawn -= OnPlayerRespawn;
        }

        private void OnPlayerDeath()
        {
            // Pause the player.
            CursorLocker.AddCursorUnlocker(this);

            // Disable the Character Controller.
            CharacterController characterController = Character.gameObject.GetComponent<CharacterController>();
            characterController.enabled = false;

            // Stop inventory inspection.
            if (TryGetModule(out IInventoryInspectManager inventoryInspectManager))
                inventoryInspectManager.StopInspection();

            // Handle item dropping.
            if (TryGetModule(out IItemDropHandler itemDropHandler))
                HandleItemDrop(itemDropHandler);

            // Handle wieldable dropping.
            if (TryGetModule(out IWieldableSelectionHandler wieldableSelection))
                HandleWieldableDrop(wieldableSelection);

            // Holster Weapon.
            if (TryGetModule(out IWieldablesController wieldablesController))
                wieldablesController.TryEquipWieldable(null, 1.3f);

            // Do death module effects.
            if (TryGetModule(out IDeathModule deathModule))
                deathModule.DoDeathEffects(Character);
        }

        private void OnPlayerRespawn() 
        {
            // Unpause the player.
            CursorLocker.RemoveCursorUnlocker(this);

            // Do death module respawn effects.
            if (TryGetModule(out IDeathModule deathModule))
                deathModule.DoRespawnEffects(Character);

            // Reset Thirst.
            if (TryGetModule(out IThirstManager thirst))
                thirst.Thirst = thirst.MaxThirst;

            // Reset Energy.
            if (TryGetModule(out IEnergyManager energy))
                energy.Energy = energy.MaxEnergy;

            // Reset Hunger.
            if (TryGetModule(out IHungerManager hunger))
                hunger.Hunger = hunger.MaxHunger;

            StartCoroutine(C_EnableController());
        }

        private IEnumerator C_EnableController() 
        {
            yield return new WaitForEndOfFrame();

            // Re-enable the Character Controller.
            CharacterController characterController = Character.gameObject.GetComponent<CharacterController>();
            characterController.enabled = true;

            // Reset the player's state.
            if (TryGetModule(out IMovementController movement))
                movement.ResetController();
        }

        private void HandleItemDrop(IItemDropHandler dropHandler) 
        {
            if (m_ItemDropType == ItemDropType.All)
            {
                foreach (var container in Character.Inventory.Containers)
                {
                    foreach (var slot in container.Slots)
                    {
                        if (slot.HasItem)
                            dropHandler.DropItem(slot);
                    }
                }
            }
        }

        private void HandleWieldableDrop(IWieldableSelectionHandler wieldableSelection)
        {
            if (m_ItemDropType == ItemDropType.Equipped || m_ItemDropType == ItemDropType.All)
                wieldableSelection.DropWieldable();
        }
    }
}
