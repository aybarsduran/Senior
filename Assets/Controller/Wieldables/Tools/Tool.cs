using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Tool")]
    public class Tool : Wieldable, IUseInputHandler
    {
        public ActionBlockHandler UseBlocker { get; private set; } = new ActionBlockHandler();

        [SerializeField, Range(0f, 10f)]
        private float m_UseCooldown = 0.3f;

        [SerializeField]
        private EffectCollection m_UseEffects;

        private float m_NextTimeCanUse;


        public void Use(UsePhase usePhase)
        {
            if (IsEquipping() || UseBlocker.IsBlocked || m_NextTimeCanUse > Time.time || usePhase != UsePhase.Start)
                return;

            m_NextTimeCanUse = Time.time + m_UseCooldown;
            m_UseEffects.PlayEffects(this);
        }
    }
}
