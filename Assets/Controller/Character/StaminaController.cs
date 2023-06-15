using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public sealed class StaminaController : CharacterBehaviour, IStaminaController
    {
        #region Internal
        [System.Serializable]
        private class StaminaState
        {
            public MovementStateType StateType;

            [Range(-1f, 1f)]
            public float EnterChange;

            [Range(-1f, 1f)]
            public float ExitChange;

            [Range(-1f, 1f)]
            public float ChangeRatePerSec;


            public StaminaState(MovementStateType stateType, float enterChange, float exitChange, float changeRatePerSec)
            {
                StateType = stateType;
                EnterChange = enterChange;
                ExitChange = exitChange;
                ChangeRatePerSec = changeRatePerSec;
            }
        }
        #endregion

        public float Stamina
        {
            get => m_Stamina;
            set
            {
                if (m_Stamina != value)
                {
                    if (value < m_Stamina)
                        m_NextAllowedRegenTime = Time.time + m_RegenerationPause;

                    m_Stamina = Mathf.Clamp01(value);
                    StaminaChanged?.Invoke(m_Stamina);

                    OnStaminaChanged();
                }
            }
        }

        public event UnityAction<float> StaminaChanged;

        [SerializeField, Range(0f, 5f)]
        [Tooltip("How much time the stamina regeneration will be paused after it gets lowered.")]
        private float m_RegenerationPause = 3f;

        [SpaceArea]

        [SerializeField]
        [ReorderableList(ListStyle.Boxed, childLabel: "StateType")]
        private StaminaState[] m_StaminaStates;

        [SpaceArea]

        [SerializeField, Range(0.1f, 25f)]
        private float m_BreathingHeavyDuration;

        [SerializeField]
        private StandardSound m_BreathingHeavyAudio;

#if UNITY_EDITOR
        [SerializeField, Disable, SpaceArea]
#endif
        private float m_Stamina = 1f;

        private float m_NextAllowedRegenTime;
        private float m_LastHeavyBreathTime;

        private StaminaState m_CurrentState;
        private IMovementController m_Movement;

        private static readonly StaminaState s_DefaultState = new(MovementStateType.Idle, 0f, 0f, 0.2f);


        protected override void OnBehaviourEnabled()
        {
            m_Stamina = 1f;

            GetModule(out m_Movement);
            m_Movement.StateChanged += OnStateChanged;

            m_CurrentState = GetStateOfType(m_Movement.ActiveState);

            UpdateManager.AddFixedUpdate(UpdateStamina);
        }

        protected override void OnBehaviourDisabled()
        {
            m_Movement.StateChanged -= OnStateChanged;
            UpdateManager.RemoveFixedUpdate(UpdateStamina);
        }

        private void UpdateStamina(float deltaTime)
        {
            // Decrease stamina.
            if (m_CurrentState.ChangeRatePerSec < 0f)
                Stamina += m_CurrentState.ChangeRatePerSec * deltaTime;

            // Regenerate stamina.
            else if (Time.time > m_NextAllowedRegenTime)
                Stamina += m_CurrentState.ChangeRatePerSec * deltaTime;
        }

        private void OnStateChanged()
        {
            Stamina += m_CurrentState.ExitChange;
            m_CurrentState = GetStateOfType(m_Movement.ActiveState);
            Stamina += m_CurrentState.EnterChange;
        }

        private void OnStaminaChanged()
        {
            if (Time.time - m_LastHeavyBreathTime < m_BreathingHeavyDuration)
                return;

            if (m_Stamina < 0.01f)
            {
                Character.AudioPlayer.LoopSound(m_BreathingHeavyAudio, m_BreathingHeavyDuration);
                m_LastHeavyBreathTime = Time.time;
            }
        }

        private StaminaState GetStateOfType(MovementStateType stateType) 
        {
            for (int i = 0; i < m_StaminaStates.Length; i++)
            {
                if (m_StaminaStates[i].StateType == stateType)
                    return m_StaminaStates[i];
            }

            return s_DefaultState;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetStateOfType(MovementStateType.Idle) == null)
                UnityEditor.ArrayUtility.Add(ref m_StaminaStates, s_DefaultState);
        }
#endif
    }
}