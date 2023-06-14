using System;
using System.Collections.Generic;
using UnityEngine;

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


    }
}
