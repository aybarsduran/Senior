using IdenticalStudios.InventorySystem;
using IdenticalStudios.WorldManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.BuildingSystem
{
    /// <summary>
    /// Convert the cooking to item actions.
    /// </summary>
    public sealed class CookingStation : Workstation, ISaveableComponent
    {
        #region Internal
        private class CookingSlot
        {
            public bool CanCook => Property != null;

            public IItem Item;
            public CookData Data;
            public IItemProperty Property;
        }
        #endregion

        public bool CookingActive
        {
            get => m_CookingIsActive;
            private set
            {
                if (m_CookingIsActive != value)
                {
                    m_CookingIsActive = value;

                    if (m_CookingIsActive)
                    {
                        CookingStarted?.Invoke();
                        StartCoroutine(C_Update());
                    }
                    else
                    {
                        StopAllCoroutines();
                        CookingStopped?.Invoke();
                    }
                }
            }
        }

        public float CookingStrength => m_CookingDurationRealtime / m_MaxTemperatureAchieveTime;

        public event UnityAction CookingStarted;
        public event UnityAction CookingStopped;
        public event UnityAction<float> FuelAdded;

        [Title("Settings (Cooking)")]

        [DisableInPlayMode]
        [SerializeField, Range(1, 10)]
        [Tooltip("How many cooking spots (item slots) this campfire has.")]
        private int m_CookingSpots = 3;

        [SerializeField, Range(0.01f, 10f)]
        [Tooltip("Multiplies the effects of any fuel added (heat and added time).")]
        private float m_FuelDurationMod = 1f;

        [SerializeField, Range(0.01f, 10f)]
        [Tooltip("Multiplies the cooking speed.")]
        private float m_CookingSpeedMod = 1f;

        [SpaceArea]

        [SerializeField]
        [Tooltip("The property that tells the campfire how cooked an item is.")]
        private DataIdReference<ItemPropertyDefinition> m_CookedAmountProperty;

        [Title("Temperature (Cooking)")]

        [SerializeField, Range(1f, 1000f)]
        private float m_MaxTemperatureAchieveTime = 500;

        [SerializeField, Range(40, 60)]
        private int m_MaxProximityTemperature = 50;

        private float m_InGameDayScale;

        private float m_CookingDurationRealtime;
        private bool m_CookingIsActive = false;

        private ItemContainer m_Container;
        private CookingSlot[] m_CookingSlots;


        public override IItemContainer[] GetContainers() => new IItemContainer[] { m_Container };

        public void StartCooking(float fuelDuration)
        {
            m_CookingDurationRealtime = Mathf.Clamp(m_CookingDurationRealtime + fuelDuration * m_FuelDurationMod, 0f, m_MaxTemperatureAchieveTime);
            CookingActive = true;
        }

        public void StopCooking()
        {
            m_CookingDurationRealtime = 0f;
            Description = string.Empty;
            CookingActive = false;
        }

        public void AddFuel(float fuelDuration)
        {
            m_CookingDurationRealtime = Mathf.Clamp(m_CookingDurationRealtime + fuelDuration * m_FuelDurationMod, 0f, m_MaxTemperatureAchieveTime);
            FuelAdded?.Invoke(fuelDuration);
        }

        private void Start()
        {
            m_CookingSlots = new CookingSlot[m_CookingSpots];
            for (int i = 0; i < m_CookingSpots; i++)
                m_CookingSlots[i] = new();

            // How much smaller is the game day vs a realtime day (a value of 1 means the day takes 1440 real minutes, like the real day takes)
            m_InGameDayScale = WorldManagerBase.HasInstance ? WorldManagerBase.Instance.GetDayDurationInMinutes() / 1440f : WorldManagerBase.k_DefaultDayDurationInMinutes / 1440f;

            if (m_Container == null)
                GenerateContainer();
        }

        private void GenerateContainer()
        {
            m_Container = new ItemContainer("Cooking", m_CookingSpots, new ContainerDataRestriction(typeof(CookData)), new ContainerPropertyRestriction(m_CookedAmountProperty));
            m_Container.ContainerChanged += OnCookingContainerChanged;
        }

        private void OnCookingContainerChanged()
        {
            for (int i = 0; i < m_CookingSpots; i++)
            {
                var cookingSlot = m_CookingSlots[i];
                var item = m_Container[i].Item;
                cookingSlot.Item = item;

                if (item == null)
                {
                    cookingSlot.Data = null;
                    cookingSlot.Property = null;
                }
                else
                {
                    var cookData = item.Definition.GetDataOfType<CookData>();

                    cookingSlot.Data = cookData;
                    cookingSlot.Property = item.GetPropertyWithId(m_CookedAmountProperty);
                }
            }
        }

        private void UpdateCooking(float deltaTime)
        {
            for (int i = 0; i < m_CookingSpots; i++)
            {
                if (!m_CookingSlots[i].CanCook)
                    continue;

                var slot = m_CookingSlots[i];
                var property = slot.Property;
                var stackCount = slot.Item.StackCount;
                var data = slot.Data;

                property.Float += 1f / stackCount * data.CookTime * m_CookingSpeedMod * deltaTime;

                if (property.Float >= 1f)
                {
                    if (!data.CookedOutput.IsNull)
                        m_Container[i].Item = new Item(data.CookedOutput.Def, stackCount);
                    else
                        m_Container[i].Item = null;
                }
            }
        }

        private void UpdateDescriptionText()
        {
            GameTime fireDuration = new(m_CookingDurationRealtime, m_InGameDayScale);
            string infoString = $"Duration: {fireDuration.GetTimeToStringWithSuffixes(true, true, false)} \n"; 
            infoString += $"Heat: +{Mathf.RoundToInt(CookingStrength * m_MaxProximityTemperature)}C";

            Description = infoString;
        }

        private IEnumerator C_Update()
        {
            yield return null;

            while (m_CookingIsActive)
            {
                if (m_CookingDurationRealtime < 0f)
                    StopCooking();
                else
                    m_CookingDurationRealtime -= Time.deltaTime;

                UpdateCooking(Time.deltaTime);

                if (InspectionActive || HoverActive)
                    UpdateDescriptionText();

                yield return null;
            }
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_Container = (ItemContainer)members[0];
            m_Container.OnLoad();
            m_CookingDurationRealtime = (float)members[1];
            CookingActive = (bool)members[2];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_Container,
                m_CookingDurationRealtime,
                m_CookingIsActive,
            };

            return members;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            m_CookedAmountProperty = new DataIdReference<ItemPropertyDefinition>(ItemPropertyDefinition.GetWithName("Cooked Amount"));
        }
#endif
        #endregion
    }
}