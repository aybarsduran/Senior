using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CameraMotionEnablerEffect : WieldableEffect
    {
        [FormerlySerializedAs("m_Motions")]
        [SerializeReference]
        [ReorderableList(ListStyle.Boxed, childLabel: "Motion")]
        private MotionData[] m_OverridesToEnable = Array.Empty<MotionData>();

        private IMotionDataHandler m_DataHandler;


        public CameraMotionEnablerEffect() { }

        public CameraMotionEnablerEffect(params MotionData[] motions)
        {
            m_OverridesToEnable = motions;
        }

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_DataHandler = wieldable.Character.GetModule<ICameraMotionHandler>().DataHandler;
        }

        public override void PlayEffect()
        {
            for (int i = 0; i < m_OverridesToEnable.Length; i++)
                m_DataHandler.AddDataOverride(m_OverridesToEnable[i]);
        }
    }
}
