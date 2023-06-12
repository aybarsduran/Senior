using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    public class WieldableMotionWeightEffect : WieldableEffect
    {
        [SerializeField, Range(0f, 1f)]
        private float m_Weight = 1f;

        [SerializeField]
        [ReorderableList(ListStyle.Boxed, HasLabels = false)]
        private SerializedType[] m_Motions = Array.Empty<SerializedType>();

        private IMixedMotion[] m_MixedMotions;


        public WieldableMotionWeightEffect() { }

        public WieldableMotionWeightEffect(float weight, params Type[] motions)
        {
            m_Weight = weight;

            m_Motions = new SerializedType[motions.Length];
            for (int i = 0; i < m_Motions.Length; i++)
                m_Motions[i] = new SerializedType(motions[i]);
        }

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            var mixer = wieldable.gameObject.GetComponentInFirstChildren<FPWieldableMotionMixer>();
            m_MixedMotions = new IMixedMotion[m_Motions.Length];

            for (int i = 0; i < m_Motions.Length; i++)
                m_MixedMotions[i] = mixer.GetMotionOfType(m_Motions[i]);

#if !UNITY_EDITOR
            m_Motions = null;
#endif
        }

        public override void PlayEffect()
        {
            for (int i = 0; i < m_MixedMotions.Length; i++)
                m_MixedMotions[i].WeightMod = m_Weight;
        }
    }
}
