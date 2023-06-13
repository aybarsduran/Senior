using IdenticalStudios.BuildingSystem;
using IdenticalStudios.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(IWieldablesController))]
    public sealed class WieldableSurvivalBookHandler : CharacterBehaviour, IWieldableSurvivalBookHandler, ISaveableComponent
    {
        public bool InspectionActive
        {
            get => m_InspectionActive;
            set
            {
                if (value != m_InspectionActive)
                {
                    m_InspectionActive = value;
                    m_NextTimeCanToggle = Time.time + m_ToggleCooldown;

                    if (m_InspectionActive)
                    {
                        InspectionStarted?.Invoke();
                    }
                    else
                    {
                        InspectionEnded?.Invoke();
                    }
                }
            }
        }

        public IWieldable AttachedWieldable => m_SurvivalBookWieldable;
        public Transform LeftPages { get; private set; }
        public Transform RightPages { get; private set; }

        public event UnityAction InspectionStarted;
        public event UnityAction InspectionEnded;

        [SerializeField]
        private InputContextGroup m_BookContext;

        [Title("Settings")]

        [SerializeField, PrefabObjectOnly]
        [Tooltip("Corresponding survival book wieldable prefab.")]
        private Wieldable m_SurvivalBookPrefab;

        [SerializeField]
        [Tooltip("The left pages object name of the book (used to parent the UI).")]
        private string m_LeftPagesObjectName;

        [SerializeField]
        [Tooltip("The right pages object name of the book (used to parent the UI).")]
        private string m_RightPagesObjectName;

        [SpaceArea]

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How often can the survival book be Enabled/disabled (in seconds).")]
        private float m_ToggleCooldown = 0.75f;

        [SerializeField, HideInInspector]
        private bool m_InspectionActive = false;

        private float m_NextTimeCanToggle = -1f;

        private IWieldablesController m_WieldableController;
        private IWieldableSelectionHandler m_SelectionHandler;
        private IBuildingController m_BuildingController;
        private IWieldable m_SurvivalBookWieldable;


        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_InspectionActive = (bool)members[0];
        }

        public object[] SaveMembers()
        {
            return new object[] { m_InspectionActive };
        }
        #endregion

        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_SelectionHandler);
            GetModule(out m_WieldableController);
            GetModule(out m_BuildingController);

            m_SurvivalBookWieldable = m_WieldableController.AddWieldable(m_SurvivalBookPrefab);

            LeftPages = m_SurvivalBookWieldable.transform.FindDeepChild(m_LeftPagesObjectName);
            RightPages = m_SurvivalBookWieldable.transform.FindDeepChild(m_RightPagesObjectName);

            if (m_InspectionActive)
                TryStartInspection();
        }

        public bool TryStartInspection()
        {
            if (Time.time < m_NextTimeCanToggle || InputManager.HasEscapeCallbacks)
                return false;

            return m_WieldableController.TryEquipWieldable(m_SurvivalBookWieldable, 1.3f, OnEquipped);

            void OnEquipped()
            {
                InspectionActive = true;

                CursorLocker.AddCursorUnlocker(this);
                InputManager.PushEscapeCallback(ForceEndInspection);
                InputManager.PushContext(m_BookContext);
            }
        }

        public bool TryStopInspection(BuildableDefinition buildableDef)
        {
            if (Time.time < m_NextTimeCanToggle || !InputManager.HasEscapeCallbacks)
                return false;

            if (buildableDef == null)
                ForceEndInspection();
            else
            {
                StopInspection();
                m_WieldableController.TryEquipWieldable(null);
                m_BuildingController.SetBuildable(buildableDef);
            }

            return true;
        }

        private void ForceEndInspection()
        {
            m_SelectionHandler.SelectAtIndex(m_SelectionHandler.SelectedIndex);
            StopInspection();
        }

        private void StopInspection()
        {
            InspectionActive = false;

            CursorLocker.RemoveCursorUnlocker(this);
            InputManager.PopEscapeCallback(ForceEndInspection);
            InputManager.PopContext(m_BookContext);
        }
    }
}