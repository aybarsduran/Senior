using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class MotionDataHandler : CharacterBehaviour, IMotionDataHandler
    {
        public bool IsVisualizing
        {
            get
            {
#if UNITY_EDITOR
                return m_StateToVisualize != null;
#else
                return false;
#endif
            }
        }

        public MotionPreset DefaultPreset => m_DefaultPreset;

        public event MotionDataChangedDelegate Changed;
        
        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(PresetChanged))]
#endif
        private MotionPreset m_DefaultPreset;

        private IMovementController m_Movement;
        private MotionPreset m_ActivePreset;
        private MovementStateType m_StateType;

        private readonly Dictionary<Type, MotionData> m_DataOverrides = new();
        
        
        public void SetPreset(MotionPreset preset)
        {
#if UNITY_EDITOR
            if (m_ActivePreset != null)
                m_ActivePreset.onPresetChanged -= ForceUpdate;
#endif

            m_ActivePreset = preset != null ? preset : m_DefaultPreset;

#if UNITY_EDITOR
            if (m_ActivePreset != null)
                m_ActivePreset.onPresetChanged += ForceUpdate;
#endif

            Changed?.Invoke(false);

#if UNITY_EDITOR
            void ForceUpdate() => Changed?.Invoke(true);
#endif
        }

        public void ForceRefreshData() => Changed?.Invoke(true);

        public void AddDataOverride(MotionData data)
        {
            if (data == null)
                return;

            var dataType = data.GetType();
            m_DataOverrides.Add(dataType, data);

            Changed?.Invoke(false);
        }

        public void RemoveDataOverride(MotionData data)
        {
            if (data == null)
                return;
         
            var dataType = data.GetType();

            if (m_DataOverrides.Remove(dataType))
                Changed?.Invoke(false);
        }

        public void RemoveDataOverride(Type dataType)
        {
            if (dataType == null)
                return;

            if (m_DataOverrides.Remove(dataType))
                Changed?.Invoke(false);
        }

        public T GetData<T>() where T : MotionData
        {
            if (m_DataOverrides.Count > 0)
            {
                if (m_DataOverrides.TryGetValue(typeof(T), out var dataOverride))
                    return (T)dataOverride;
            }

            if (m_ActivePreset == null)
                return null;

            if (m_ActivePreset.TryGetData<T>(m_StateType, out var data))
                return data;

            if (m_ActivePreset.TryGetData(out data))
                return data;

            return null;
        }

        public bool TryGetData<T>(out T data) where T : MotionData
        {
            if (m_DataOverrides.Count > 0)
            {
                if (m_DataOverrides.TryGetValue(typeof(T), out var dataOverride))
                {
                    data = (T)dataOverride;
                    return true;
                }
            }
            
            if (m_ActivePreset == null)
            {
                data = null;
                return false;
            }

            if (m_ActivePreset.TryGetData(m_StateType, out data))
                return true;

            if (m_ActivePreset.TryGetData(out data))
                return true;

            return false;
        }

        protected override void OnBehaviourEnabled()
        {
            SetPreset(m_DefaultPreset);

            GetModule(out m_Movement);
            m_Movement.StateChanged += OnStateChanged;
        }

        protected override void OnBehaviourDisabled()
        {
            m_Movement.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged()
        {
            m_StateType = m_Movement.ActiveState;
            
#if UNITY_EDITOR
            if (m_StateToVisualize != null)
                m_StateType = m_StateToVisualize.Value;
#endif
            
            Changed?.Invoke(false);
        }
        
#if UNITY_EDITOR
        private MovementStateType? m_StateToVisualize;


        public void Visualize(MovementStateType? stateToVisualize)
        {
            if (!Application.isPlaying)
                return;

            m_StateToVisualize = stateToVisualize;
            OnStateChanged();
        }

        public void PresetChanged()
        {
            if (Application.isPlaying)
                SetPreset(m_DefaultPreset);
        }
#endif
    }
}
