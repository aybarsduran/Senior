using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(MeleeComboHandler))]
    public sealed class MeleeWeapon : Wieldable, IUseInputHandler
    {
        public ActionBlockHandler UseBlocker { get; } = new ActionBlockHandler();

        [SerializeField]
        private bool m_SwingContinuously = true;

        [SerializeField]
        private MeleeComboHandler m_ComboHandler;

        private IStaminaController m_Stamina;
        private const float k_MinStaminaCondition = 0.05f;


        public void Use(UsePhase usePhase)
        {
            if (usePhase == UsePhase.End || (!m_SwingContinuously && usePhase == UsePhase.Hold) || IsEquipping() || !IsVisible)
                return;

            if (!UseBlocker.IsBlocked && (m_Stamina == null || m_Stamina.Stamina > k_MinStaminaCondition))
            {
                // Do swing and lower stamina.
                if (m_ComboHandler.TryMelee(Accuracy) && m_Stamina != null)
                {
                    float staminaToDeplete = m_ComboHandler.LastMeleeSwing.AttackEffort;
                    m_Stamina.Stamina -= staminaToDeplete;
                }
            }
        }

        public override void OnHolster(float holsterSpeed)
        {
            base.OnHolster(holsterSpeed);
            m_ComboHandler.CancelAllSwings();
        }

        protected override void OnCharacterChanged(ICharacter character)
        {
            m_Stamina = character.GetModule<IStaminaController>();
        }

        protected override void Awake()
        {
            base.Awake();
            Weight = 1f;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_ComboHandler = GetComponent<MeleeComboHandler>();
        }
#endif
    }
}