using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public class FuelData : ItemData
    {
        public float FuelEfficiency => Random.Range(m_MinFuel, m_MaxFuel);

        [SerializeField, Range(0f, 1000f)]
        private float m_MinFuel = 30f;

        [SerializeField, Range(0f, 1000f)]
        private float m_MaxFuel = 40f;
    }
}
