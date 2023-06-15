using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
	[RequireComponent(typeof(AdditiveForceMotionModule))]
	[AddComponentMenu("IdenticalStudios/Motion/Bob Motion")]
    public sealed class BobMotionModule : DataMotionModule<BobMotionDataBase>
	{
		[SerializeField]
		private AdditiveForceMotionModule m_ForceModule;

		[Title("Interpolation")]

		[SerializeField, Range(0.1f, 10f)]
		private float m_ResetSpeed = 1f;

		[SerializeField, Range(0f, 3.14f)]
		private float m_RotationDelay = 0.5f;

		private ICharacterMotor m_Motor;
		private IMovementController m_Movement;

		private float m_BobParam;
		private Vector3 m_PosBob;
		private Vector3 m_RotBob;
		private bool m_Inversed;

		private const float k_PositionBobMod = 0.01f;
		private const float k_RotationBobMod = 0.5f;
		

		protected override void OnBehaviourEnabled()
        {
	        base.OnBehaviourEnabled();
	        
			GetModule(out m_Motor);
			GetModule(out m_Movement);

			m_Movement.StepCycleEnded += OnStepped;
		}

		protected override void OnBehaviourDisabled()
		{
			base.OnBehaviourDisabled();

			m_Movement.StepCycleEnded -= OnStepped;
		}
		
		protected override BobMotionDataBase GetDataFromPreset(IMotionDataHandler dataHandler)
		{
			if (dataHandler.TryGetData(out SimpleBobMotionData data))
				return data;

			return dataHandler.GetData<BobMotionData>();
		}

		protected override void OnDataChanged(BobMotionDataBase data)
		{
			if (data != null)
			{
				positionSpring.Adjust(data.PositionSettings);
				rotationSpring.Adjust(data.RotationSettings);
			}
		}

        public override void TickMotionLogic(float deltaTime)
		{
			bool canUpdateBob = CanUpdateBob();
			if (rotationSpring.IsIdle && !canUpdateBob)
				return;

			// Update the bob parameter.
			if (canUpdateBob)
			{
				switch (Data.BobType)
                {
                    case BobMotionDataBase.BobMode.StepCycleBased:
                        m_BobParam = m_Movement.StepCycle * Mathf.PI;
                        m_BobParam += m_Inversed ? 0f : Mathf.PI;
                        break;
                    case BobMotionDataBase.BobMode.TimeBased:
						m_BobParam = Time.time * Data.BobSpeed;
						break;
                }

                // Calculate the position bob.
                Vector3 posAmplitude = Data.PositionAmplitude;
				m_PosBob = new Vector3
				{
					x = Mathf.Cos(m_BobParam) * posAmplitude.x,
					y = Mathf.Cos(m_BobParam * 2) * posAmplitude.y,
					z = Mathf.Cos(m_BobParam) * posAmplitude.z
				};

				// Calculate the rotation bob.
				Vector3 rotAmplitude = Data.RotationAmplitude;
				m_RotBob = new Vector3
				{
					x = Mathf.Cos(m_BobParam * 2 + m_RotationDelay) * rotAmplitude.x,
					y = Mathf.Cos(m_BobParam + m_RotationDelay) * rotAmplitude.y,
					z = Mathf.Cos(m_BobParam + m_RotationDelay) * rotAmplitude.z
				};
			}
			// Reset bob...
			else
			{
				float resetSpeed = Data == null ? deltaTime * 50f : deltaTime * m_ResetSpeed;

				m_BobParam = Mathf.MoveTowards(m_BobParam, 0f, resetSpeed);

				if (Mathf.Abs(m_PosBob.x + m_PosBob.y + m_PosBob.y) > 0.001f)
					m_PosBob = Vector3.MoveTowards(m_PosBob, Vector3.zero, resetSpeed);
				else
					m_PosBob = Vector3.zero;

				if (Mathf.Abs(m_RotBob.x + m_RotBob.y + m_RotBob.y) > 0.001f)
					m_RotBob = Vector3.MoveTowards(m_RotBob, Vector3.zero, resetSpeed);
				else
					m_RotBob = Vector3.zero;
			}

			SetTargetPosition(m_PosBob, k_PositionBobMod);
			SetTargetRotation(m_RotBob, k_RotationBobMod);
		}

		private void OnStepped()
		{
			if (!CanUpdateBob())
				return;
			
			float stepFactor = m_Motor.Velocity.sqrMagnitude < 0.1f ? 0f : 1f;

			m_ForceModule.AddPositionForce(Data.PositionStepForce, 0.05f * stepFactor * Weight);
			m_ForceModule.AddRotationForce(Data.RotationStepForce, stepFactor * Weight);

			m_Inversed = !m_Inversed;
		}
		
		private bool CanUpdateBob()
		{
			if (Data == null)
				return false;

			return Data.BobType == BobMotionDataBase.BobMode.TimeBased ||
				(m_Motor.IsGrounded && (m_Motor.Velocity.sqrMagnitude > 0.1f || m_Motor.TurnSpeed > 0.1f));
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_ForceModule == null)
				m_ForceModule = GetComponent<AdditiveForceMotionModule>();
		}
#endif
	}
}