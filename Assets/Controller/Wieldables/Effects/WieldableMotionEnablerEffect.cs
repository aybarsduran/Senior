using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public sealed class WieldableMotionEnablerEffect : WieldableEffect
    {
        [SerializeReference]
        [ReorderableList(ListStyle.Boxed, childLabel: "Motion")]
        private MotionData[] m_OverridesToEnable = Array.Empty<MotionData>();

        private IMotionDataHandler m_DataHandler;


        public WieldableMotionEnablerEffect() { }

        public WieldableMotionEnablerEffect(params MotionData[] motions)
        {
            m_OverridesToEnable = motions;
        }

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_DataHandler = wieldable.gameObject.GetComponentInFirstChildren<FPWieldableMotionMixer>().DataHandler;
        }

        public override void PlayEffect()
        {
#if UNITY_EDITOR
            if (m_DataHandler == null)
            {
                IsInitialized = false;
                return;
            }
#endif

            for (int i = 0; i < m_OverridesToEnable.Length; i++)
                m_DataHandler.AddDataOverride(m_OverridesToEnable[i]);
        }
    }
}
