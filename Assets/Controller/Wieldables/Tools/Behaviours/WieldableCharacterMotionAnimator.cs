using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class WieldableCharacterMotionAnimator : WieldableBehaviour
    {
        #region Internal
        [Serializable]
        private class AnimationTrigger
        {
            public MovementStateType State;
            public TriggerType TriggerType;

            [SerializeField]
            private AnimatorParameterTrigger[] m_Parameters;

            private Animator m_Animator;


            public void Initialize(Animator animator)
            {
                m_Animator = animator;
                for (int i = 0; i < m_Parameters.Length; i++)
                    m_Parameters[i].HashParameter();
            }

            public void Trigger()
            {
                for (int i = 0; i < m_Parameters.Length; i++)
                    m_Parameters[i].TriggerParameter(m_Animator);
            }
        }

        [Serializable]
        private enum TriggerType
        {
            StateEnter,
            StateExit
        }
        #endregion

        [SerializeField]
        private WieldableAnimator m_WieldableAnimator;


        [SerializeField, ReorderableList]
        private AnimationTrigger[] m_AnimationTriggers;

        private IMovementController m_Movement;


        protected override void OnEquippingStarted()
        {
            if (m_Movement != null || Wieldable.Character.TryGetModule(out m_Movement))
            {
                for (int i = 0; i < m_AnimationTriggers.Length; i++)
                {
                    if (m_AnimationTriggers[i].TriggerType == TriggerType.StateEnter)
                        m_Movement.AddStateEnterListener(m_AnimationTriggers[i].State, m_AnimationTriggers[i].Trigger);
                    else
                        m_Movement.AddStateExitListener(m_AnimationTriggers[i].State, m_AnimationTriggers[i].Trigger);
                }
            }
        }

        protected override void OnHolsteringEnded()
        {
            if (m_Movement != null)
            {
                for (int i = 0; i < m_AnimationTriggers.Length; i++)
                {
                    if (m_AnimationTriggers[i].TriggerType == TriggerType.StateEnter)
                        m_Movement.RemoveStateEnterListener(m_AnimationTriggers[i].State, m_AnimationTriggers[i].Trigger);
                    else
                        m_Movement.RemoveStateExitListener(m_AnimationTriggers[i].State, m_AnimationTriggers[i].Trigger);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (var trigger in m_AnimationTriggers)
                trigger.Initialize(m_WieldableAnimator.Animator);
        }
    }
}
