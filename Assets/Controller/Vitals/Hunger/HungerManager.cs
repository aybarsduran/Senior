using UnityEngine;

namespace IdenticalStudios
{
    public sealed class HungerManager : CharacterVitalModule, IHungerManager, ISaveableComponent
    {
        public float Hunger
        {
            get => m_Hunger;
            set => m_Hunger = Mathf.Clamp(value, 0f, m_MaxHunger);
        }

        public float MaxHunger
        {
            get => m_MaxHunger;
            set
            {
                m_MaxHunger = Mathf.Max(value, 0f);
                m_Hunger = Mathf.Clamp(m_Hunger, 0f, m_MaxHunger);
            }
        }

#if UNITY_EDITOR
        [SerializeField, Disable, SpaceArea]
#endif
        private float m_Hunger;

#if UNITY_EDITOR
        [SerializeField, Disable]
#endif
        private float m_MaxHunger;


        protected override void Awake()
        {
            base.Awake();

            InitalizeStat(ref m_Hunger, ref m_MaxHunger);
        }

        private void FixedUpdate()
        {
            if (m_HealthManager.IsAlive)
                DepleteStat(ref m_Hunger, m_MaxHunger, Time.fixedDeltaTime);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_Hunger = (float)members[0];
            m_MaxHunger = (float)members[1];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_Hunger,
                m_MaxHunger
            };

            return members;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            MaxHunger = m_InitialMaxValue;
            Hunger = m_InitialValue;
        }
#endif
        #endregion
    }
}