using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class AnimationWieldableEffect : WieldableEffect
    {        
        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private AnimatorParameterTrigger[] m_Parameters;
        
        [NonSerialized]
        private WieldableAnimator m_Animator;


        public AnimationWieldableEffect()
        {
        }

        public AnimationWieldableEffect(params AnimatorParameterTrigger[] paramTriggers)
        {
            m_Parameters = paramTriggers;
        }

        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_Animator = wieldable.gameObject.GetComponentInFirstChildren<WieldableAnimator>();

            for (int i = 0; i < m_Parameters.Length; i++)
                m_Parameters[i].HashParameter();
        }

        public override void PlayEffect()
        {
            var animator = m_Animator.Animator;
            foreach (var parameter in m_Parameters)
                parameter.TriggerHashedParameter(animator);
        }

        public override void PlayEffectDynamically(float value)
        {
            var animator = m_Animator.Animator;
            foreach (var parameter in m_Parameters)
                parameter.TriggerHashedParameter(animator, value);
        }
    }
}