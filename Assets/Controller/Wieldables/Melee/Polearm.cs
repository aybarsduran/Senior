using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(MeleeComboHandler))]
    [AddComponentMenu("IdenticalStudios/Wieldables/Melee/Polearm")]
    public sealed class Polearm : Wieldable, IAimInputHandler, IUseInputHandler
    {
        public bool IsAiming => m_BaseAimer.IsAiming;

        public ActionBlockHandler AimBlocker { get; private set; } = new ActionBlockHandler();
        public ActionBlockHandler UseBlocker { get; } = new ActionBlockHandler();

        [Title("Polearm")]

        [SerializeField]
        private bool m_SwingContinuously = true;

        [SerializeField]
        private MeleeComboHandler m_ComboHandler;

        [SerializeField, NotNull]
        private FirearmAimerBehaviour m_BaseAimer;

        private float m_NextTimeCanUse;
        private IStaminaController m_Stamina;
        private const float k_MinStaminaCondition = 0.05f;


        public void Use(UsePhase usePhase)
        {
            if (usePhase == UsePhase.End || (!m_SwingContinuously && usePhase == UsePhase.Hold) || IsEquipping() || !IsVisible || m_NextTimeCanUse > Time.time)
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
        
        public void StartAiming()
        {
            if (!AimBlocker.IsBlocked && !IsEquipping())
            {
                m_BaseAimer.StartAim();
                m_NextTimeCanUse = Time.time + 0.3f;
            }
        }

        public void EndAiming()
        {
            m_BaseAimer.EndAim();
            m_NextTimeCanUse = Time.time + 0.3f;
        }

        public override void OnHolster(float holsterSpeed)
        {
            base.OnHolster(holsterSpeed);
            EndAiming();
        }

        protected override void Awake()
        {
            base.Awake();

            Weight = 1f;
            AimBlocker.SetBlockCallback(EndAiming);
        }

        protected override void OnCharacterChanged(ICharacter character)
        {
            m_Stamina = character.GetModule<IStaminaController>();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_ComboHandler = GetComponent<MeleeComboHandler>();
        }

        private void OnValidate()
        {
            if (m_BaseAimer != null)
                m_BaseAimer.AttachOnStart = true;
        }
#endif
    }
}