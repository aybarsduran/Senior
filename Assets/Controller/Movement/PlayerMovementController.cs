using IdenticalStudios.InputSystem.Behaviours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.MovementSystem
{
    public sealed class PlayerMovementController : CharacterBehaviour, IMovementController, ISaveableComponent
    {
        public MovementStateType ActiveState => m_ActiveState != null ? m_ActiveState.StateType : MovementStateType.Idle;

        public MovementModifier VelocityModifier { get; } = new();
        public MovementModifier AccelerationModifier { get; } = new();
        public MovementModifier DecelerationModifier { get; } = new();

        public float StepCycle => m_StepCycle;

        public event UnityAction StateChanged;
        public event UnityAction StepCycleEnded;
        public event UnityAction<MovementPreset, MovementPreset> PresetChanged;

        [SerializeField, NotNull]
        private FPSMovementInput m_InputHandler;

        [SerializeField, NotNull]
        private MovementPreset m_DefaultPreset;

        [Title("Step Cycle")]

        [Tooltip("How fast will the transtion between different step lengths happen (used mainly in determining when to footsteps).")]
        [SerializeField, Range(0.1f, 10f)]
        private float m_StepLerpSpeed = 1f;

        [SerializeField, Range(0f, 10f)]
        private float m_TurnStepLength = 1f;

        [SerializeField, Range(0f, 10f)]
        private float m_MaxTurnStepVelocity = 2f;

        private MovementPreset m_ActivePreset;
        private ICharacterMotionState m_ActiveState;
        private MovementStateType m_StartingStateType = MovementStateType.Idle;

        private Dictionary<MovementStateType, ICharacterMotionState> m_StatesDict;
        private readonly Dictionary<MovementStateType, List<Object>> m_StateLockers = new();
        private readonly Dictionary<MovementStateType, UnityAction> m_StateEnterEvents = new();
        private readonly Dictionary<MovementStateType, UnityAction> m_StateExitEvents = new();

        private ICharacterMotor m_CMotor;
        private UnityAction m_TempActionHolder;
        private float m_DistMovedSinceLastCycleEnded;
        private float m_CurrentStepLength;
        private float m_StepCycle;


        #region Initialization
        protected override void OnBehaviourEnabled()
        {
            if (m_DefaultPreset == null)
            {
                Debug.LogError("The default movement preset cannot be null. You need to create a new one or assign an existing preset.");
                return;
            }

            GetModule(out m_CMotor);
            m_CMotor.SetMotionInput(GetMotionInput);

            SetMovementPreset(m_DefaultPreset);
        }
        #endregion

        #region Preset Changing
        public void SetMovementPreset(MovementPreset preset)
        {
            if (preset == null)
                preset = m_DefaultPreset;

            if (m_ActivePreset == preset)
                return;

            // Decommisions the previous preset.
            if (m_ActivePreset != null)
            {
                foreach (var state in m_StatesDict.Values)
                    state.DecommisionState();

                VelocityModifier.Remove(preset.GetSpeedMultiplier);
                AccelerationModifier.Remove(preset.GetAccelaration);
                DecelerationModifier.Remove(preset.GetDeceleration);
            }

            // Initializes the new preset.
            m_StatesDict = preset.GetAllStates();
            foreach (var state in m_StatesDict.Values)
                state.InitializeState(this, m_InputHandler, m_CMotor, Character);

            VelocityModifier.Add(preset.GetSpeedMultiplier);
            AccelerationModifier.Add(preset.GetAccelaration);
            DecelerationModifier.Add(preset.GetDeceleration);

            var prevPreset = m_ActivePreset;
            m_ActivePreset = preset;

            ForceSetState(GetStateOfMotionType(m_StartingStateType));

            PresetChanged?.Invoke(prevPreset, preset);
        }

        public void ResetController()
        {
            m_InputHandler.ResetAllInputs();
            SetMovementPreset(m_DefaultPreset);
        }
        #endregion

        #region State Change Events
        public void AddStateEnterListener(MovementStateType stateType, UnityAction callback)
        {
            if (m_StateEnterEvents.TryGetValue(stateType, out m_TempActionHolder))
            {
                m_TempActionHolder += callback;
                m_StateEnterEvents[stateType] = m_TempActionHolder;
            }
            else
                m_StateEnterEvents.Add(stateType, callback);
        }

        public void AddStateExitListener(MovementStateType stateType, UnityAction callback)
        {
            if (m_StateExitEvents.TryGetValue(stateType, out m_TempActionHolder))
            {
                m_TempActionHolder += callback;
                m_StateExitEvents[stateType] = m_TempActionHolder;
            }
            else
                m_StateExitEvents.Add(stateType, callback);
        }

        public void RemoveStateEnterListener(MovementStateType stateType, UnityAction callback)
        {
            if (m_StateEnterEvents.TryGetValue(stateType, out m_TempActionHolder))
            {
                m_TempActionHolder -= callback;
                m_StateEnterEvents[stateType] = m_TempActionHolder;
            }
        }

        public void RemoveStateExitListener(MovementStateType stateType, UnityAction callback)
        {
            if (m_StateExitEvents.TryGetValue(stateType, out m_TempActionHolder))
            {
                m_TempActionHolder -= callback;
                m_StateExitEvents[stateType] = m_TempActionHolder;
            }
        }
        #endregion

        #region State Accessing
        public ICharacterMotionState GetStateOfMotionType(MovementStateType stateType) => m_StatesDict[stateType];

        public T GetStateOfType<T>() where T : ICharacterMotionState
        {
            foreach (var state in m_StatesDict.Values)
            {
                if (state.GetType() == typeof(T))
                    return (T)state;
            }

            return default;
        }
        #endregion

        #region State Changing
        public bool TrySetState(ICharacterMotionState state) 
        {
            if (state == null || m_ActiveState == state || !state.Enabled || !state.IsStateValid())
                return false;

            // Handles state previous state exit.
            if (m_ActiveState != null)
            {
                m_ActiveState.OnStateExit();

                if (m_StateExitEvents.TryGetValue(m_ActiveState.StateType, out var stateExitEvent))
                    stateExitEvent();
            }

            // Handles next state enter.
            var prevStateType = ActiveState;
            m_ActiveState = state;
            m_ActiveState.OnStateEnter(prevStateType);

            if (m_StateEnterEvents.TryGetValue(state.StateType, out var stateEnterEvent))
                stateEnterEvent();

            StateChanged?.Invoke();

            return true;
        }

        public bool TrySetState(MovementStateType stateType)
        {
            var state = GetStateOfMotionType(stateType);
            return TrySetState(state);
        }

        private void ForceSetState(ICharacterMotionState state)
        {
            // Handles state previous state exit.
            if (m_ActiveState != null)
            {
                m_ActiveState.OnStateExit();

                if (m_StateExitEvents.TryGetValue(m_ActiveState.StateType, out var stateExitEvent))
                    stateExitEvent();
            }

            // Handles next state enter.
            var prevStateType = ActiveState;
            m_ActiveState = state;
            m_ActiveState.OnStateEnter(prevStateType);

            if (m_StateEnterEvents.TryGetValue(state.StateType, out var stateEnterEvent))
                stateEnterEvent();

            StateChanged?.Invoke();
        }
        #endregion

        #region Update Loop
        private Vector3 GetMotionInput(Vector3 velocity, out bool useGravity, out bool snapToGround)
        {
            float deltaTime = Time.deltaTime;

            useGravity = m_ActiveState.ApplyGravity;
            snapToGround = m_ActiveState.SnapToGround;
            var newVelocity = m_ActiveState.UpdateVelocity(velocity, deltaTime);

            // Update the step cycle, mainly used for footsteps
            UpdateStepCycle(deltaTime);

            m_ActiveState.UpdateLogic();

            return newVelocity;
        }
        #endregion

        #region State Locking
        public void AddStateLocker(Object locker, MovementStateType stateType)
        {
            List<Object> list;

            // Gets existing locker list for the given state type if available
            if (m_StateLockers.TryGetValue(stateType, out list))
            {
                int prevCount = list.Count;

                list.Add(locker);

                if (prevCount == 0 && list.Count != prevCount)
                    DisableStateOfType(stateType);
            }
            // Creates a new locker list for the given state type
            else
            {
                list = new List<Object> { locker };
                m_StateLockers.Add(stateType, list);

                DisableStateOfType(stateType);
            }
        }

        public void RemoveStateLocker(Object locker, MovementStateType stateType)
        {
            // Gets existing locker list for the given state type if available
            if (m_StateLockers.TryGetValue(stateType, out var list))
            {
                list.Remove(locker);

                if (list.Count == 0)
                    EnableStateOfType(stateType);
            }
        }

        private void EnableStateOfType(MovementStateType stateType) 
        {
            if (m_StatesDict.TryGetValue(stateType, out var state))
                state.Enabled = true;
        }

        private void DisableStateOfType(MovementStateType stateType)
        {
            if (m_StatesDict.TryGetValue(stateType, out var state))
                state.Enabled = false;
        }
        #endregion

        #region Step Cycle
        private void UpdateStepCycle(float deltaTime)
        {
            if (!m_CMotor.IsGrounded)
                return;

            // Advance the step cycle based on the current velocity.
            m_DistMovedSinceLastCycleEnded += m_CMotor.Velocity.magnitude * deltaTime;
            float targetStepLength = Mathf.Max(m_ActiveState.StepCycleLength, 1f);
            m_CurrentStepLength = Mathf.MoveTowards(m_CurrentStepLength, targetStepLength, deltaTime * m_StepLerpSpeed);

            // Advance the step cycle based on the character turn.
            m_DistMovedSinceLastCycleEnded += Mathf.Clamp(m_CMotor.TurnSpeed, 0f, m_MaxTurnStepVelocity) * deltaTime * m_TurnStepLength;

            // If the step cycle is complete, reset it, and send a notification.
            if (m_DistMovedSinceLastCycleEnded > m_CurrentStepLength)
            {
                m_DistMovedSinceLastCycleEnded -= m_CurrentStepLength;
                StepCycleEnded?.Invoke();
            }

            m_StepCycle = m_DistMovedSinceLastCycleEnded / m_CurrentStepLength;
        }
        #endregion

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_StartingStateType = (MovementStateType)members[0];
        }

        public object[] SaveMembers()
        {
            return new object[] { ActiveState };
        }
        #endregion
    }
}