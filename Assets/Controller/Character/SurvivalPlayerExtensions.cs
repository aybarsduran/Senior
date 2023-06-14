using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
    /// Utility class mainly used for prototyping.
    /// </summary>
    public static class SurvivalPlayerExtensions
    {
        public static IWieldablesController GetWieldableController(this ICharacter character) => character.GetModule<IWieldablesController>();
        public static IWieldableSelectionHandler GetWieldableInventory(this ICharacter character) => character.GetModule<IWieldableSelectionHandler>();
        public static ICharacterMotor GetCharacterMotor(this ICharacter character) => character.GetModule<ICharacterMotor>();
        public static IMovementController GetMovementController(this ICharacter character) => character.GetModule<IMovementController>();
        public static IItemDropHandler GetItemDropHandler(this ICharacter character) => character.GetModule<IItemDropHandler>();

        public static void SetPositionAndRotation(this ICharacter character, Vector3 position, Quaternion rotation) => character.GetCharacterMotor().Teleport(position, rotation);
        public static void DropWeapon(this ICharacter character, float dropDelay = 0f) => character.GetItemDropHandler().DropItem(character.GetWieldableInventory().ItemSlot, dropDelay);

        public static void SetPlayerHealth(this ICharacter character, float health)
        {
            float currentHealth = character.HealthManager.Health;

            if (currentHealth < health)
                character.HealthManager.RestoreHealth(health - currentHealth);
            else
                character.HealthManager.ReceiveDamage(currentHealth - health);
        }
    }
}