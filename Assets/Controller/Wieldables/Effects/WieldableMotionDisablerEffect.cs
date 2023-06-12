using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class WieldableMotionDisablerEffect : WieldableEffect
    {
        [FormerlySerializedAs("m_DataTypeToDisable")]
        [SerializeField]
        [ReorderableList(ListStyle.Boxed, HasLabels = false)]
        private SerializedType[] m_OverridesToDisable = Array.Empty<SerializedType>();

        private IMotionDataHandler m_DataHandler;


        public WieldableMotionDisablerEffect() { }

        public WieldableMotionDisablerEffect(params Type[] dataTypesToDisable)
        {
            m_OverridesToDisable = new SerializedType[dataTypesToDisable.Length];
            for (int i = 0; i < dataTypesToDisable.Length; i++)
                m_OverridesToDisable[i] = new SerializedType(dataTypesToDisable[i]);
        }

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_DataHandler = wieldable.GetComponentInChildren<FPWieldableMotionMixer>(true).DataHandler;
        }

        public override void PlayEffect()
        {
            for (int i = 0; i < m_OverridesToDisable.Length; i++)
                m_DataHandler.RemoveDataOverride(m_OverridesToDisable[i]);
        }
    }
}
