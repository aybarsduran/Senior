using UnityEngine;

namespace IdenticalStudios
{
    public sealed class ThirstManager : CharacterVitalModule, IThirstManager, ISaveableComponent
    {
        public float Thirst
        {
            get => m_Thirst;
            set => m_Thirst = Mathf.Clamp(value, 0f, m_MaxThirst);
        }

        public float MaxThirst
        {
            get => m_MaxThirst;
            set
            {
                m_MaxThirst = Mathf.Max(value, 0f);
                Thirst = Mathf.Clamp(Thirst, 0f, m_MaxThirst);
            }
        }

        [SerializeField]
        private float m_Thirst;

        [SerializeField]
        private float m_MaxThirst;


        protected override void Awake()
        {
            base.Awake();
            InitalizeStat(ref m_Thirst, ref m_MaxThirst);
        }

        private void FixedUpdate()
        {
            if (m_HealthManager.IsAlive)
                DepleteStat(ref m_Thirst, m_MaxThirst, Time.fixedDeltaTime);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_Thirst = (float)members[0];
            m_MaxThirst = (float)members[1];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_Thirst,
                m_MaxThirst
            };

            return members;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            MaxThirst = m_InitialMaxValue;
            Thirst = m_InitialValue;
        }
#endif
        #endregion
    }
}