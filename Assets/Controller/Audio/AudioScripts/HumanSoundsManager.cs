using System;
using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
	/// TODO: Refactor
    /// Plays sounds based on the current state of a humanoid character.
    /// </summary>
	public sealed class HumanSoundsManager : CharacterBehaviour
    {
	
		[Serializable]
		private struct MotionStateAudio
		{
			public MovementStateType StateType;
			public StandardSound EnterAudio;
			public StandardSound ExitAudio;

			[SpaceArea]

			public bool PlayLoopAudio;

			[ShowIf(nameof(PlayLoopAudio), true)]
			public StandardSound LoopAudio;
		}

		[Serializable]
		private struct DamageAudio
		{
			[Range(0f, 100f), Tooltip("")]
			public float DamageAmountThreshold;

			[Tooltip("The sounds that will be played when this entity receives damage.")]
			public StandardSound GenericDamageAudio;

			[Range(0f, 50f), Tooltip("")]
			public float FallDamageSpeedThreshold;

			public StandardSound FallDamageAudio;
			public StandardSound DeathAudio;

			[Range(0f, 100f), Tooltip("")]
			public float HeartbeatHealthThreshold;

			public StandardSound HeartbeatAudio;
		}

		[SerializeField]
		[ReorderableList(ListStyle.Boxed, childLabel: "StateType")]
		private MotionStateAudio[] m_Motions;

		[SpaceArea]

		[SerializeField]
		private DamageAudio m_Damage;

		private IAudioPlayer m_AudioPlayer;
		private IMovementController m_Movement;
		private ICharacterMotor m_Motor;
		private IHealthManager m_HealthManager;

		private bool m_HeartbeatActive;


		protected override void OnBehaviourEnabled()
        {
			GetModule(out m_Movement);
			GetModule(out m_Motor);
			GetModule(out m_AudioPlayer);
			GetModule(out m_HealthManager);

			m_Motor.FallImpact += OnFallImpact;
			m_HealthManager.Death += OnDeath;
            m_HealthManager.HealthRestored += OnHealthRestored;
			m_HealthManager.DamageTaken += OnDamageTaken;

			CreateMotionAudioEvents();
		}

        protected override void OnBehaviourDisabled()
        {
			m_Motor.FallImpact -= OnFallImpact;
			m_HealthManager.Death -= OnDeath;
			m_HealthManager.HealthRestored -= OnHealthRestored;
			m_HealthManager.DamageTaken -= OnDamageTaken;
		}

        private void CreateMotionAudioEvents() 
		{
			foreach (var motion in m_Motions)
			{
				m_Movement.AddStateEnterListener(motion.StateType, () =>
				{
					m_AudioPlayer.PlaySound(motion.EnterAudio);

					if (motion.PlayLoopAudio)
						m_AudioPlayer.LoopSound(motion.LoopAudio, 0f);
				});

				m_Movement.AddStateExitListener(motion.StateType, () =>
				{
					m_AudioPlayer.PlaySound(motion.ExitAudio);

					if (motion.PlayLoopAudio)
						m_AudioPlayer.StopLoopingSound(motion.LoopAudio, 0.1f);
				});
			}
		}

        private void OnDeath() => m_AudioPlayer.PlaySound(m_Damage.DeathAudio);

		private void OnFallImpact(float impactSpeed)
		{
			// Don't play the clip when the impact speed is low
			bool wasHardImpact = Mathf.Abs(impactSpeed) >= m_Damage.FallDamageSpeedThreshold;
			wasHardImpact &= m_HealthManager.Health < m_HealthManager.PrevHealth;

			if (wasHardImpact)
				m_AudioPlayer.PlaySound(m_Damage.FallDamageAudio);
		}

		private void OnHealthRestored(float arg0)
		{
			// Stop heartbeat loop sound...
			if (m_HeartbeatActive && m_HealthManager.Health > m_Damage.HeartbeatHealthThreshold)
			{
				m_AudioPlayer.StopLoopingSound(m_Damage.HeartbeatAudio);
				m_HeartbeatActive = false;
			}
		}

		private void OnDamageTaken(float damage)
		{
			if (damage > m_Damage.DamageAmountThreshold)
				m_AudioPlayer.PlaySound(m_Damage.GenericDamageAudio);

			// Start heartbeat loop sound...
			if (!m_HeartbeatActive && m_HealthManager.Health < m_Damage.HeartbeatHealthThreshold)
			{
				m_AudioPlayer.LoopSound(m_Damage.HeartbeatAudio, 1000f);
				m_HeartbeatActive = true;
			}
		}


    }
}