using IdenticalStudios.ProceduralMotion;
using IdenticalStudios;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IdenticalStudios.ProceduralMotion
{
    [CreateAssetMenu(menuName = "IdenticalStudios/Motion/Motion Preset", fileName = "(Motion) ", order = 100)]
    public class MotionPreset : ScriptableObject
    {
        #region Internal
        [Serializable]
        public struct StateData
        {
            public MovementStateType State;

            //[SerializeReference, ReorderableList(ListStyle.Boxed, "Data"), ReferencePicker]
            [SerializeReference, ReorderableList(ListStyle.Boxed, "Data")]
            public MotionData[] Data;
        }
        #endregion

        [SerializeField]

        private MotionPreset m_BasePreset;

        [SerializeField]
        [FormerlySerializedAs("m_StateData")]

        private StateData[] m_MotionData;

        private Dictionary<int, Dictionary<Type, MotionData>> m_StatesDict;


        public bool TryGetData<T>(MovementStateType stateType, out T motionData) where T : MotionData
        {
            if (m_StatesDict == null)
                InitializeData();

            if (m_StatesDict.TryGetValue((int)stateType, out var dictionary))
            {
                if (dictionary.TryGetValue(typeof(T), out var data))
                {
                    motionData = (T)data;
                    return true;
                }
            }

            motionData = null;
            return m_BasePreset != null && m_BasePreset.TryGetData(stateType, out motionData);
        }

        public bool TryGetData<T>(out T motionData) where T : MotionData
        {
            if (m_StatesDict == null)
                InitializeData();

            if (m_StatesDict.TryGetValue(-1, out var noneDict) && noneDict.TryGetValue(typeof(T), out var data))
            {
                motionData = (T)data;
                return true;
            }

            motionData = null;
            return m_BasePreset != null && m_BasePreset.TryGetData(out motionData);
        }

        private void InitializeData()
        {
            m_StatesDict = new Dictionary<int, Dictionary<Type, MotionData>>();
            foreach (var state in m_MotionData)
            {
                int stateType = (int)state.State;
                if (!m_StatesDict.ContainsKey(stateType))
                {
                    var dict = new Dictionary<Type, MotionData>();

                    foreach (var data in state.Data)
                    {
                        if (data != null)
                            dict.Add(data.GetType(), data);
                    }

                    m_StatesDict.Add(stateType, dict);
                }
            }
        }

#if UNITY_EDITOR
        public event UnityAction onPresetChanged;

        public StateData GetNewState()
        {
            return new StateData() { Data = null, State = MovementStateType.None };
        }

        public void PresetChanged()
        {
            if (m_BasePreset == this)
                m_BasePreset = null;

            if (m_BasePreset != null && m_BasePreset.m_BasePreset == this)
                m_BasePreset = null;
        }

        private void Reset()
        {
            m_MotionData = new StateData[] { new StateData() { State = MovementStateType.None } };
        }

        private void OnValidate()
        {
            if (m_MotionData == null)
                return;

            foreach (var state in m_MotionData)
            {
                var data = state.Data;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == null && i != data.Length - 1)
                    {
                        UnityEditor.ArrayUtility.RemoveAt(ref data, i);
                        i--;
                    }
                }
            }

            if (Application.isPlaying)
            {
                InitializeData();
                onPresetChanged?.Invoke();
            }
        }
#endif
    }
}