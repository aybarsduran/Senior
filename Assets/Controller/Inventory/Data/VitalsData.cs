using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public class VitalsData : ItemData
    {
        public float HealthChange => Random.Range(m_MinHealthChange, m_MaxHealthChange);
        public float HungerChange => Random.Range(m_MinHungerChange, m_MaxHungerChange);
        public float ThirstChange => Random.Range(m_MinThirstChange, m_MaxThirstChange);
        public float EnergyChange => Random.Range(m_MinEnergyChange, m_MaxEnergyChange);

        [Title("Health")]

        [SerializeField, Range(-300f, 300f)]
        private float m_MinHealthChange;

        [SerializeField, Range(-300f, 300f)]
        private float m_MaxHealthChange;

        [Title("Hunger")]

        [SerializeField, Range(-300f, 300f)]
        private float m_MinHungerChange;

        [SerializeField, Range(-300f, 300f)]
        private float m_MaxHungerChange;

        [Title("Thirst")]

        [SerializeField, Range(-300f, 300f)]
        private float m_MinThirstChange;

        [SerializeField, Range(-300f, 300f)]
        private float m_MaxThirstChange;

        [Title("Energy")]

        [SerializeField, Range(-300f, 300f)]
        private float m_MinEnergyChange;

        [SerializeField, Range(-300f, 300f)]
        private float m_MaxEnergyChange;
    }
}