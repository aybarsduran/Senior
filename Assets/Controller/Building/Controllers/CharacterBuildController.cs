using IdenticalStudios.InputSystem;
using IdenticalStudios.ProceduralMotion;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.BuildingSystem
{
    /// <summary>
    /// Handles placing and building objects.
    /// </summary>
    public class CharacterBuildController : CharacterBehaviour, IBuildingController
    {
        public bool IsBuildingActive => Buildable != null;
        public BuildingMode Mode => m_PlacementState == m_SocketPlacement ? BuildingMode.Socket : BuildingMode.Free;
        public BuildableDefinition Buildable { get; private set; }
        public float RotationOffset { get; set; }

        public event UnityAction BuildingStarted
        {
            add => m_OnBuildingStart.AddListener(value);
            remove => m_OnBuildingStart.RemoveListener(value);
        }

        public event UnityAction BuildingStopped
        {
            add => m_OnBuildingEnd.AddListener(value);
            remove => m_OnBuildingEnd.RemoveListener(value);
        }

        public event UnityAction ObjectPlaced
        {
            add => m_OnPlaceObject.AddListener(value);
            remove => m_OnPlaceObject.RemoveListener(value);
        }

        public event UnityAction<BuildableDefinition> BuildableChanged;

        [SerializeField]
        private InputContextGroup m_BuildingContext;


        [SerializeField]
        private FreePlacementState m_FreePlacement;

        [SerializeField]
        private StructurePlacementState m_SocketPlacement;

        //Effects

        [SerializeField]
        [Tooltip("Sound to play when the controller tries to place an object but detects a collision.")]
        private StandardSound m_InvalidPlaceSound;

        [SerializeField]
        private ShakeSettings3D m_CameraShake;

        [SerializeField]
        private CameraEffectSettings m_CameraEffect;

        //Events

        [SerializeField]
        private UnityEvent m_OnPlaceObject;

        [SerializeField]
        private UnityEvent m_OnBuildingStart;

        [SerializeField]
        private UnityEvent m_OnBuildingEnd;

        private float m_NextTimeCanPlace;
        private ILookHandler m_LookHandler;
        private ICameraEffectsHandler m_CameraEffects;

        private PlacementState m_PlacementState;


        protected override void OnBehaviourEnabled()
        {
            InitializeBuildStates();

            GetModule(out m_LookHandler);
            GetModule(out m_CameraEffects);
        }

        public void SetBuildable(BuildableDefinition buildable)
        {
            if (buildable == Buildable)
                return;
            
            if (m_PlacementState.Buildable != null)
                Destroy(m_PlacementState.Buildable.gameObject);

            if (Buildable == null && buildable != null)
            {
                m_OnBuildingStart.Invoke();
                InputManager.PushContext(m_BuildingContext);
                InputManager.PushEscapeCallback(ForceEndBuilding);
                m_LookHandler.PostViewUpdate += UpdateObjectPlacement;
            }
            else if (Buildable != null && buildable == null)
            {
                m_OnBuildingEnd.Invoke();
                InputManager.PopContext(m_BuildingContext);
                InputManager.PopEscapeCallback(ForceEndBuilding);
                m_LookHandler.PostViewUpdate -= UpdateObjectPlacement;
            }

            Buildable = buildable;
            m_PlacementState = GetPlacementStateForBuildable(buildable);

            BuildableChanged?.Invoke(buildable);

            void ForceEndBuilding() => SetBuildable(null);
            void UpdateObjectPlacement() => m_PlacementState.UpdatePlacement(RotationOffset);
        }

        public void SelectNextBuildable(bool next)
        {
            var nextBuildable = m_PlacementState.SelectNextBuildable(next);
            if (nextBuildable != Buildable)
                SetBuildable(nextBuildable);
        }

        public void PlaceBuildable()
        {
            if (Time.time < m_NextTimeCanPlace)
                return;

            if (m_PlacementState.TryPlaceActiveBuildable())
            {
                if (m_PlacementState.ContinueBuildingOnPlaced)
                    m_PlacementState.TrySetBuildable(CreateBuildable(Buildable));
                else
                {
                    m_PlacementState.TrySetBuildable(null);
                    SetBuildable(null);
                }

                // Play place effects.
                CameraShakeManagerBase.DoShake(m_CameraShake, 1f);
                m_CameraEffects.DoAnimationEffect(m_CameraEffect);

                m_OnPlaceObject.Invoke();
            }
            else
            {
                // Play invalid place sound.
                Character.AudioPlayer.PlaySound(m_InvalidPlaceSound);
                m_NextTimeCanPlace = Time.time + 0.5f;
            }
        }

        private void InitializeBuildStates()
        {
            m_PlacementState = m_FreePlacement;
            m_FreePlacement.Initialize(Character);
            m_SocketPlacement.Initialize(Character);
        }

        private PlacementState GetPlacementStateForBuildable(BuildableDefinition buildableDef)
        {
            if (buildableDef == null)
            {
                m_FreePlacement.TrySetBuildable(null);
                return m_FreePlacement;
            }

            var floatingBuildable = CreateBuildable(buildableDef);

            if (m_SocketPlacement.TrySetBuildable(floatingBuildable))
                return m_SocketPlacement;

            if (m_FreePlacement.TrySetBuildable(floatingBuildable))
                return m_FreePlacement;

            return null;
        }

        private static Buildable CreateBuildable(BuildableDefinition definition)
        {
            if (definition == null)
                return null;

            var floatingBuildable = Instantiate(definition.Prefab);
            floatingBuildable.OnCreated();

            return floatingBuildable;
        }
    }
}