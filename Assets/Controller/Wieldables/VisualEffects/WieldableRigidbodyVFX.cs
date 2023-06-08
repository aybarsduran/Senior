using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class WieldableRigidbodyVFX : WieldableVFXBehaviour<Rigidbody>
	{
		[SerializeField, Range(0, 100f)]
		private float m_Spin = 0.3f;

		[SerializeField, Range(0, 100f)]
		private float m_Speed = 2f;

		[SerializeField]
		private Vector3 m_SpeedJitter = Vector3.one * 0.2f;


        protected override Rigidbody SpawnEffect()
        {
            var obj = base.SpawnEffect();

			Vector3 velocityJitter = new(Random.Range(-m_SpeedJitter.x, m_SpeedJitter.x),
				Random.Range(-m_SpeedJitter.y, m_SpeedJitter.y),
				Random.Range(-m_SpeedJitter.z, m_SpeedJitter.z));

			Vector3 velocity = (Character.GetModule<ICharacterMotor>().Velocity * 0.15f) + SpawnRoot.TransformVector(Vector3.forward * m_Speed + velocityJitter);

			float spinDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
			float spinAmount = Random.Range(0.7f, 1.3f) * m_Spin;

			obj.velocity = velocity;
			obj.maxAngularVelocity = 10000f;
			obj.angularVelocity = spinAmount * spinDirection * Vector3.one;

			return obj;
		}
    }
}