using IdenticalStudios.ProceduralMotion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/FP Motion")]
    public class FPWieldableMotionMixer : MotionMixer
    {
        public IMotionDataHandler DataHandler { get; set; }
        public MotionPreset MotionPreset => m_MotionPreset;

        [SerializeField]
        private MotionPreset m_MotionPreset;


        public void SetMotions(List<IMixedMotion> motions, Dictionary<Type, IMixedMotion> motionsDict)
        {
            m_Motions = motions ?? s_EmptyList;
            m_MotionsDict = motionsDict;
        }
    }
}