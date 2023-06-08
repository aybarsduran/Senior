using IdenticalStudios.PoolingSystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class WieldableVFXBehaviour : WieldableBehaviour
    {
        public abstract void PlayEffect();
    }

    public abstract class WieldableVFXBehaviour<T> : WieldableVFXBehaviour where T : Component
    {
		protected Transform SpawnRoot => m_SpawnRoot;

		private enum RotationMode { Local, Random }
		private enum RootFollowMode { OnSpawn, Continuously }


		[SerializeField]
        private T m_Prefab;

		[SerializeField]
		private Transform m_SpawnRoot;

		[SerializeField, Range(0f, 10f)]
		private float m_SpawnDelay = 0f;

		[SerializeField]
		private RootFollowMode m_RootFollowMode = RootFollowMode.OnSpawn;

		[SerializeField]
		private RotationMode m_RotationMode = RotationMode.Local;

		[SerializeField]
		private Vector3 m_PositionOffset;

		[SerializeField]
		private Vector3 m_RotationOffset;

		[SerializeField, Range(1, 100)]
		private int m_PoolSize = 6;

		[SerializeField, Range(1f, 60f)]
		private float m_RecycleDelay = 2f;


		public override void PlayEffect()
		{
			if (m_SpawnDelay > 0.01f)
				UpdateManager.InvokeDelayedAction(this, () => SpawnEffect(), m_SpawnDelay);
			else
				SpawnEffect();
		}

		protected virtual T SpawnEffect()
		{
			var position = m_SpawnRoot.position + m_SpawnRoot.TransformVector(m_PositionOffset);

			var rotation = Quaternion.LookRotation(m_SpawnRoot.forward, Vector3.up) * Quaternion.Euler(m_RotationOffset);
			if (m_RotationMode == RotationMode.Random)
				rotation *= Random.rotation;

			var parent = m_RootFollowMode == RootFollowMode.Continuously ? m_SpawnRoot : null;

			return PoolingManager.GetObject(m_Prefab.gameObject, position, rotation, parent).GetComponent<T>();
		}

		protected override void Awake()
		{
			base.Awake();

			if (m_Prefab == null)
			{
				Debug.LogError($"Prefab on {gameObject.name} can't be null.");
				return;
			}

			PoolingManager.CreatePool(m_Prefab.gameObject, m_PoolSize / 3, m_PoolSize, m_RecycleDelay); 
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (m_SpawnRoot == null)
				return;

			var position = m_SpawnRoot.position + m_SpawnRoot.TransformVector(m_PositionOffset);
			var rotation = Quaternion.Euler(m_RotationOffset);

			transform.SetPositionAndRotation(position, rotation);

			Gizmos.DrawSphere(position, 0.05f);
		}
#endif
	}
}