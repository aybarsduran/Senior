using System;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [Serializable]
    public class BuildRequirement
    {
        public DataIdReference<BuildMaterialDefinition>  BuildMaterial => m_BuildMaterial;

        public int RequiredAmount
        {
            get => m_RequiredAmount;
            set
            {
                if (value == m_RequiredAmount)
                    return;
                
                m_RequiredAmount = Mathf.Clamp(value, 0, 1000);

                if (m_CurrentAmount > m_RequiredAmount)
                    CurrentAmount = m_RequiredAmount;
            }
        }

        public int CurrentAmount 
        {
            get => m_CurrentAmount;
            set
            {
                if (value != m_CurrentAmount)
                    m_CurrentAmount = Mathf.Clamp(value, 0, m_RequiredAmount);
            }
        }

        [SerializeField]
        private int m_BuildMaterial;

        [SerializeField]
        private int m_CurrentAmount;

        [SerializeField]
        private int m_RequiredAmount;


        public BuildRequirement(DataIdReference<BuildMaterialDefinition> buildMaterial, int requiredAmount, int currentAmount)
        {
            this.m_BuildMaterial = buildMaterial;
            this.m_RequiredAmount = requiredAmount;
            this.m_CurrentAmount = currentAmount;
        }

        public BuildRequirement(BuildRequirementInfo buildRequirementInfo)
        {
            this.m_BuildMaterial = buildRequirementInfo.BuildMaterial;
            this.m_RequiredAmount = buildRequirementInfo.RequiredAmount;
            this.m_CurrentAmount = 0;
        }

        public bool IsCompleted() => RequiredAmount == CurrentAmount;
    }

    [Serializable]
    public class BuildRequirementInfo : ISerializationCallbackReceiver
    {
        public DataIdReference<BuildMaterialDefinition> BuildMaterial => m_BuildMaterial;
        public int RequiredAmount => m_RequiredAmount;

        [SerializeField]
        [DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<BuildMaterialDefinition> m_BuildMaterial;

        [SerializeField, Range(1, 100)]
        private int m_RequiredAmount = 1;


        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            if (m_RequiredAmount == 0)
                m_RequiredAmount = 1;
        }
    }
}