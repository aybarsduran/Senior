using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Items/Actions/Change Vitals Action", fileName = "(Action) Change Vitals")]
    public sealed class ChangeVitalsAction : ItemAction
    {
        [Title("Eating")]

        [SerializeField, Range(0f, 10f)]
        private float m_Duration;

        [SerializeField]
        private SoundPlayer m_EatSound;


        /// <summary>
        /// Checks if the given item can be eaten.
        /// </summary>
        public override bool IsViableForItem(ICharacter character, ItemSlot itemSlot) => itemSlot.HasItem && itemSlot.Item.Definition.HasDataOfType(typeof(VitalsData));
        public override float GetDuration(ICharacter character, ItemSlot itemSlot) => m_Duration;

        public override void StartAction(ICharacter character, ItemSlot itemSlot) => m_EatSound.Play2D();

        public override void PerformAction(ICharacter character, ItemSlot itemSlot)
        {
            if (itemSlot.Item.Definition.TryGetDataOfType<VitalsData>(out var foodData))
            {
                if (!Mathf.Approximately(foodData.HealthChange, 0f))
                {
                    if (character.TryGetModule<IHealthManager>(out var health))
                    {
                        float healthChange = foodData.HealthChange;

                        if (healthChange > 0f)
                            health.RestoreHealth(foodData.HealthChange);
                        else
                            health.ReceiveDamage(healthChange);
                    }
                }

                if (!Mathf.Approximately(foodData.HungerChange, 0f))
                {
                    if (character.TryGetModule<IHungerManager>(out var hunger))
                        hunger.Hunger += foodData.HungerChange;
                }

                if (!Mathf.Approximately(foodData.ThirstChange, 0f))
                {
                    if (character.TryGetModule<IThirstManager>(out var thirst))
                        thirst.Thirst += foodData.ThirstChange;
                }

                if (!Mathf.Approximately(foodData.EnergyChange, 0f))
                {
                    if (character.TryGetModule<IEnergyManager>(out var energy))
                        energy.Energy += foodData.EnergyChange;
                }

                itemSlot.Item.StackCount--;
            }
        }

        public override void CancelAction(ICharacter character, ItemSlot itemSlot) { }
    }
}