using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [RequireComponent(typeof(IMotionDataHandler))]
    public abstract class DataMotionModule<DataType> : MotionModule where DataType : MotionDataBase
    {
#if UNITY_EDITOR
        protected DataType Data { get; private set; }
#else
        protected DataType Data;
#endif

        private IMotionDataHandler m_DataHandler;
        private DataType m_PresetData;
        

        protected abstract DataType GetDataFromPreset(IMotionDataHandler dataHandler);
        protected virtual void OnDataChanged(DataType data) { }

        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();

            if (m_DataHandler == null && TryGetComponent(out m_DataHandler))
                m_DataHandler.Changed += DataChanged;
        }

        protected override void OnBehaviourDisabled()
        {
            base.OnBehaviourDisabled();
            
            if (m_DataHandler != null)
                m_DataHandler.Changed -= DataChanged;

            m_DataHandler = null;
        }
        
        private void DataChanged(bool forceUpdate)
        {
            m_PresetData = GetDataFromPreset(m_DataHandler);

            var prevData = Data;
            Data = m_PresetData;

            if (Data == null)
            {
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }

            if (forceUpdate || prevData != Data)
                OnDataChanged(Data);
        }
    }
}