using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.MovementSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Movement/Movement Preset", fileName = "(Movement) ", order = 100)]
    public class MovementPreset : ScriptableObject
    {
        [SerializeField]
        [Help("Fallback preset. If a certain state is not found in this preset, the default preset will be used instead.")]
        private MovementPreset m_BasePreset;

        [SpaceArea]

        [SerializeField, Range(0.1f, 10f)]
        private float m_SpeedMultiplier = 1f;

        [SerializeField, Range(0.1f, 20f)]
        [Tooltip("How fast can this character achieve max velocity.")]
        private float m_Acceleration = 9f;

        [SerializeField, Range(0.1f, 20f)]
        [Tooltip("How fast will the character stop when there's no input (a high value will make the movement feel snappier).")]
        private float m_Deceleration = 8f;

        [SpaceArea]

        [SerializeReference, ReferencePicker]
#if UNITY_EDITOR
        [ReorderableListExposed(ListStyle.Boxed, "State", OverrideNewElementMethodName = nameof(GetNewState))]
#endif
        private CharacterMotionState[] m_States = Array.Empty<CharacterMotionState>();


        public float GetSpeedMultiplier() => m_SpeedMultiplier;
        public float GetAccelaration() => m_Acceleration;
        public float GetDeceleration() => m_Deceleration;

        public Dictionary<MovementStateType, ICharacterMotionState> GetAllStates()
        {
            var dict = new Dictionary<MovementStateType, ICharacterMotionState>();

            AddFromStates(dict, m_States);

            if (m_BasePreset != null)
                AddFromStates(dict, m_BasePreset.m_States);

            return dict;
        }

        private void AddFromStates(Dictionary<MovementStateType, ICharacterMotionState> dict, ICharacterMotionState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                var state = states[i];
                if (!dict.ContainsKey(state.StateType))
                    dict.Add(state.StateType, state);
            }
        }

#if UNITY_EDITOR
        public CharacterMotionState GetNewState() => null;

        private void OnValidate()
        {
            for (int i = 0; i < m_States.Length; i++)
            {
                if (m_States[i] == null)
                    continue;

                var stateType = m_States[i].GetType();
                for (int j = m_States.Length - 1; j > i; j--)
                {
                    if (m_States[j] == null)
                        continue;

                    if (m_States[j].GetType() == stateType)
                        ArrayUtility.RemoveAt(ref m_States, j);
                }
            }

            for (int i = 0; i < m_States.Length; i++)
            {
                if (m_States[i] != null)
                    m_States[i].OnEditorValidate();
            }
        }
#endif
    }
}
