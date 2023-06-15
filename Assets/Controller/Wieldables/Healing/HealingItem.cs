using IdenticalStudios.WieldableSystem.Effects;
using System.Collections;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class HealingItem : Wieldable, IUseInputHandler
    {
        public ActionBlockHandler UseBlocker { get; private set; } = new ActionBlockHandler();

        [SerializeField, Range(0.1f, 5f)]
        private float m_HealDuration = 2f;

        [SerializeField, Range(0f, 100f)]
        private float m_MinHealAmount = 40f;

        [SerializeField, Range(0f, 100f)]
        private float m_MaxHealAmount = 50f;

        [Title("Effects")]

        [SerializeField]
        private EffectCollection m_HealEffects;

        private bool m_IsHealing;


        public void Use(UsePhase usePhase)
        {
            if (m_IsHealing)
                return;

            m_IsHealing = true;
            StartCoroutine(C_Heal());
        }

        private IEnumerator C_Heal()
        {
            yield return new WaitForSeconds(m_HealDuration);

            float healingAmount = Random.Range(m_MinHealAmount, m_MaxHealAmount);
            Character.HealthManager.RestoreHealth(healingAmount);

            m_HealEffects.PlayEffects(this);
            m_IsHealing = false;
        }
    }
}
