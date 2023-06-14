using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    public class CharacterVitalsVelocityHandler : CharacterBehaviour
    {
        [SerializeField, Range(0f, 20f)]
        private float m_InterpolationSpeed = 1f;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("How much will the max velocity be affected by low vitals (e.g. hunger, thirst etc.)")]
        private float m_LowVitalsVelocityMod = 1f;

        private IEnergyManager m_EnergyManager;
        private IHungerManager m_HungerManager;
        private IThirstManager m_ThirstManager;
        private float m_VelocityMod = 1f;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_EnergyManager);
            GetModule(out m_HungerManager);
            GetModule(out m_ThirstManager);

            var movement = GetModule<IMovementController>();
            movement.VelocityModifier.Add(GetVelocityMod);
        }

        protected override void OnBehaviourDisabled()
        {
            var movement = GetModule<IMovementController>();
            movement.VelocityModifier.Remove(GetVelocityMod);
        }

        private float GetVelocityMod()
        {
            float targetVelocityMod = 1f;
            targetVelocityMod -= (m_EnergyManager.MaxEnergy - m_EnergyManager.Energy) / (m_EnergyManager.MaxEnergy * 10f);
            targetVelocityMod -= (m_ThirstManager.MaxThirst - m_ThirstManager.Thirst) / (m_ThirstManager.MaxThirst * 10f);
            targetVelocityMod -= (m_HungerManager.MaxHunger - m_HungerManager.Hunger) / (m_HungerManager.MaxHunger * 10f);

            m_VelocityMod = Mathf.Lerp(m_VelocityMod, targetVelocityMod, m_InterpolationSpeed * Time.deltaTime);

            return m_VelocityMod * m_LowVitalsVelocityMod;
        }
    }
}
