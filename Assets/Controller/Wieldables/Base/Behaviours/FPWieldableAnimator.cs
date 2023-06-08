using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Event Listeners/FP Wieldable Animator")]
    public class FPWieldableAnimator : WieldableAnimator
    {
        public override Animator Animator => m_Animator;
        
        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private AnimationOverrideClips m_Clips;
        
        private static readonly int s_Holster = Animator.StringToHash("Holster");


        protected override void OnEquippingStarted() => m_Animator.ResetTrigger(s_Holster);

        protected override void Awake()
        {
            base.Awake();
            m_Animator.SetClips(m_Clips);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Animator == null)
                m_Animator = GetComponentInChildren<Animator>();

            if (m_Clips != null)
            {
                var controller = m_Clips.Controller;
                if (m_Animator != null)
                    m_Animator.runtimeAnimatorController = controller;
            }
        }
#endif
    }
}