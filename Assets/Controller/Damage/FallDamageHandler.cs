using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
    /// Handles dealing fall damage to the character based on the impact velocity.
    /// </summary>
    public class FallDamageHandler : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableDamage = true;

        [SpaceArea]

        [Help("At which landing speed, the character will start taking damage.")]
        [SerializeField, Range(1f, 30f)] 
        private float m_MinFallSpeed = 12f;

        [SpaceArea]

        [Help("At which landing speed, the character will take maximum damage (die).")]
        [SerializeField, Range(1f, 50f)]
        private float m_FatalFallSpeed = 30f;


        protected override void OnBehaviourEnabled()
        {
            GetModule<ICharacterMotor>().FallImpact += OnFallImpact;
        }

        private void OnFallImpact(float impactSpeed)
        {
            if (!m_EnableDamage)
                return;

            if (impactSpeed >= m_MinFallSpeed)
                Character.HealthManager.ReceiveDamage(-100f * (impactSpeed / m_FatalFallSpeed));
        }
    }
}