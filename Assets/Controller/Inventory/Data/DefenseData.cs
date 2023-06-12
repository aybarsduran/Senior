using System;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public class DefenseData : ItemData
    {
        #region Internal
        [System.Serializable]
        private struct DefenseInfo : IEquatable<DefenseInfo>
        {
            public DamageType Type;

            [Range(0f, 10f)]
            public float Multiplier;

                
            public bool Equals(DefenseInfo other) => this.Type == other.Type;
        }
        #endregion

        [SerializeField]
        [Range(0f, 10f)]
        private float m_Modifier = 1f;

        [SpaceArea]

        [SerializeField, ReorderableList(childLabel: "Type")]
        private DefenseInfo[] m_DefenseInfo;


        /// <returns> Damage modifier for the given damage type</returns>
        public float Evaluate(DamageType dmgType)
        {
            for (int i = 0; i < m_DefenseInfo.Length; i++)
            {
                if (m_DefenseInfo[i].Type == dmgType)
                    return m_DefenseInfo[i].Multiplier * m_Modifier;
            }

            return 1f;
        }
    }
}
