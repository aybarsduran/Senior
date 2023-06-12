using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public class CookData : ItemData
    {
        public float CookTime => m_CookTime;
        public DataIdReference<ItemDefinition> CookedOutput => m_CookedOutput;

        [Title("Cooking")]

        [SerializeField, Suffix("sec")]
        private float m_CookTime = 20f;

        [SerializeField]
        private DataIdReference<ItemDefinition> m_CookedOutput;
    }
}
