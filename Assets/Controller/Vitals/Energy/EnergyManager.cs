using UnityEngine;
namespace IdenticalStudios
{
    public sealed class EnergyManager : CharacterVitalModule, IEnergyManager, ISaveableComponent
    {
        public float Energy
        {
            get => m_Energy;
            set => m_Energy = Mathf.Clamp(value, 0f, m_MaxEnergy);
        }

        public float MaxEnergy
        {
            get => m_MaxEnergy;
            set 
            {
                m_MaxEnergy = Mathf.Max(value, 0f);
                Energy = Mathf.Clamp(Energy, 0f, m_MaxEnergy);
            }
        }

#if UNITY_EDITOR
        [SerializeField, Disable, SpaceArea]
#endif
        private float m_Energy;

#if UNITY_EDITOR
        [SerializeField, Disable]
#endif
        private float m_MaxEnergy;


        protected override void Awake()
        {
            base.Awake();

            InitalizeStat(ref m_Energy, ref m_MaxEnergy);
        }

        private void FixedUpdate()
        {
            if (m_HealthManager.IsAlive)
                DepleteStat(ref m_Energy, m_MaxEnergy, Time.fixedDeltaTime);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_Energy = (float)members[0];
            m_MaxEnergy = (float)members[1];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_Energy,
                m_MaxEnergy
            };

            return members;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            MaxEnergy = m_InitialMaxValue;
            Energy = m_InitialValue;
        }
#endif
        #endregion
    }
}