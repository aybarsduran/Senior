using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios
{
    public sealed class ObjectDropper
    {
		#region Internal
		[System.Serializable]
		public sealed class DropSettings
		{
			[Title("Offset")]

			public Vector3 PositionOffset = Vector3.zero;
			public Vector3 RotationOffset = Vector3.zero;

			[FormerlySerializedAs("DropRandomness")]
			[Range(0f, 1f)]
			public float OffsetRandomness = 0.3f;

			[Title("Physics")]

			[FormerlySerializedAs("DropSpeed")]
			[Range(0f, 100f)]
			public float DropForce = 8f;

			[FormerlySerializedAs("DropAngularFactor")]
			[Range(0f, 1000f)]
			public float DropTorque = 150f;
		}
		#endregion

		private readonly ICharacter m_Character;
        private readonly Transform m_DropRoot;
		private readonly int m_Layers;


		public ObjectDropper(ICharacter character, Transform dropRoot, int layers = Physics.DefaultRaycastLayers)
		{
			m_Character = character;
			m_DropRoot = dropRoot;
			m_Layers = layers;
		}

		public T DropObject<T>(DropSettings dropSettings, T objectToDrop, float dropHeightMod) where T : Component
		{
			if (objectToDrop == null)
				return null;

			Vector3 dropOffset;
			Quaternion dropRotation = Quaternion.Lerp(Quaternion.LookRotation(m_DropRoot.forward) * Quaternion.Euler(dropSettings.RotationOffset), Random.rotationUniform, dropSettings.OffsetRandomness);
			bool dropNearObstacle = false;

			// Get object spawn offsets.
			if (Physics.Raycast(m_DropRoot.position, m_DropRoot.forward, dropSettings.PositionOffset.z * 1.1f, m_Layers, QueryTriggerInteraction.Ignore))
			{
				dropOffset = m_DropRoot.position + m_Character.transform.TransformVector(new Vector3(0f, dropSettings.PositionOffset.y * dropHeightMod, -0.2f));
				dropNearObstacle = true;
			}
			else
			{
				dropOffset = m_DropRoot.position + m_DropRoot.TransformVector(new Vector3(dropSettings.PositionOffset.x, dropSettings.PositionOffset.y * dropHeightMod, dropSettings.PositionOffset.z));
			}

			// Spawn the object.
			T droppedObject = Object.Instantiate<T>(objectToDrop, dropOffset, dropRotation);

			// Setup the rigidbody of the object.
			if (!dropNearObstacle)
			{
				if (droppedObject.TryGetComponent(out Rigidbody rigidbody))
				{
					rigidbody.isKinematic = false;

					Vector3 velocity = m_Character.GetModule<ICharacterMotor>().Velocity;
					velocity = new Vector3(velocity.x, Mathf.Abs(velocity.y), velocity.z);

					Vector3 forceVector = m_DropRoot.forward * dropSettings.DropForce + velocity;
					Vector3 torqueVector = dropRotation.eulerAngles * dropSettings.DropTorque;

					rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
					rigidbody.AddTorque(torqueVector);
				}
			}

			return droppedObject;
		}

		public GameObject DropObject(DropSettings dropSettings, GameObject objectToDrop, float dropHeightMod)
		{
			if (objectToDrop == null)
				return null;

			Vector3 dropOffset;
			Quaternion dropRotation = Quaternion.Lerp(Quaternion.LookRotation(m_DropRoot.forward) * Quaternion.Euler(dropSettings.RotationOffset), Random.rotationUniform, dropSettings.OffsetRandomness);
			bool dropNearObstacle = false;

			// Get object spawn offsets.
			if (Physics.Raycast(m_DropRoot.position, m_DropRoot.forward, dropSettings.PositionOffset.z * 1.1f, m_Layers, QueryTriggerInteraction.Ignore))
			{
				dropOffset = m_DropRoot.position + m_Character.transform.TransformVector(new Vector3(0f, dropSettings.PositionOffset.y * dropHeightMod, -0.2f));
				dropNearObstacle = true;
			}
			else
			{
				dropOffset = m_DropRoot.position + m_DropRoot.TransformVector(new Vector3(dropSettings.PositionOffset.x, dropSettings.PositionOffset.y * dropHeightMod, dropSettings.PositionOffset.z));
			}

			// Spawn the object.
			GameObject droppedObject = Object.Instantiate(objectToDrop, dropOffset, dropRotation);

			// Setup the rigidbody of the object.
			if (!dropNearObstacle)
			{
				if (droppedObject.TryGetComponent(out Rigidbody rigidbody))
				{
					rigidbody.isKinematic = false;

					Vector3 velocity = m_Character.GetModule<ICharacterMotor>().Velocity;
					velocity = new Vector3(velocity.x, Mathf.Abs(velocity.y), velocity.z);

					Vector3 forceVector = m_DropRoot.forward * dropSettings.DropForce + velocity;
					Vector3 torqueVector = dropRotation.eulerAngles * dropSettings.DropTorque;

					rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
					rigidbody.AddTorque(torqueVector);
				}
			}

			return droppedObject;
		}
    }
}