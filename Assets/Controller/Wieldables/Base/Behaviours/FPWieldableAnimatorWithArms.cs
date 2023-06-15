using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Event Listeners/FP Wieldable Animator With Arms")]
    public class FPWieldableAnimatorWithArms : WieldableAnimator
    {
        public enum AnimateMode
        {
            AnimateBoth,
            AnimateArmsOnly,
            AnimateWieldableOnly
        }

        public override Animator Animator => m_Animator;
        public AnimateMode Mode => m_Mode;

        [SerializeField]
        private AnimateMode m_Mode = AnimateMode.AnimateBoth;

        [SerializeField, NewLabel("Animator"), NotNull]
        private Animator m_WieldableAnimator;

        [SerializeField, NewLabel("Clips")]
        private AnimationOverrideClips m_WieldableClips;

        [SerializeField, NewLabel("Clips")]
        private AnimationOverrideClips m_ArmClips;

        private static readonly int s_Holster = Animator.StringToHash("Holster");
        private Animator m_Animator;


        protected override void OnEquippingStarted()
        {
            var armsHandler = Wieldable.Character.GetModule<IWieldableArmsHandler>();

            if (m_Mode == AnimateMode.AnimateArmsOnly)
                m_Animator = armsHandler.Animator;
            else
                m_Animator = m_WieldableAnimator;

            if (armsHandler != null)
            {
                armsHandler.EnableArms(true);
                armsHandler.SetParent(transform);

                if (m_Mode != AnimateMode.AnimateArmsOnly)
                    armsHandler.SyncWithAnimator(Animator);

                armsHandler.Animator.SetClips(m_ArmClips);
            }

            m_Animator.ResetTrigger(s_Holster);
        }

        protected override void OnHolsteringEnded()
        {
            if (Wieldable.Character.TryGetModule(out IWieldableArmsHandler armsHandler))
                armsHandler.EnableArms(false);
        }

        protected override void Awake()
        {
            base.Awake();
            m_WieldableAnimator.SetClips(m_WieldableClips);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_WieldableAnimator == null)
                m_WieldableAnimator = GetComponentInChildren<Animator>();

            if (m_WieldableClips != null)
            {
                var controller = m_WieldableClips.Controller;
                if (m_WieldableAnimator != null)
                    m_WieldableAnimator.runtimeAnimatorController = controller;
            }

            if (m_ArmClips != null)
            {
                if (m_Mode != AnimateMode.AnimateArmsOnly)
                    m_ArmClips.Controller = m_WieldableClips.Controller;
            }
        }
#endif
    }
}
